using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;
using VuiLen.Texas.Game.Models;
using VuiLen.TexasServer.game.Controllers.Room;
using VuiLen.TexasServer.game.Models.vos;
using VuiLenGameFramework;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.Room.Controllers;
using VuiLenGameFramework.Room.Models;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework.Implementation.Messaging;

namespace VuiLen.Texas.Game.Controllers.Room
{
    public class TexasLeaveRoomCommand : LeaveRoomCommand, TexasSitDownListener
    {
        public override void Execute(INotification notification)
        {
            var roomUserModel = Facade.RetrieveProxy(RoomUserModel.NAME) as RoomUserModel;

            Guid peerId = (Guid)notification.Body;

            Log.DebugFormat("LeaveTexasRoomCommand::" + peerId);
            var userData = roomUserModel.UserData(peerId);
            if (userData == null)
            {
                return;
            }
            var texasModel = Facade.RetrieveProxy(TexasModel.NAME) as TexasModel;

            // first remove roomUser
            Slot player = texasModel.leaveRoom(userData.UserKey, this);

            if (player == null)
            {
                base.Execute(notification);
                return;
            }

            // deposit credit
            if (player.CurrentCredit > 0)
            {
                Log.DebugFormat("LeaveTexasRoomCommand::3:" + player.CurrentCredit);
                var param = new Dictionary<byte, object>()
                {
                    { (byte)RoomParameterCode.GameCredit, player.CurrentCredit},
                    { (byte)ServerParameterCode.PeerId, peerId.ToByteArray()},
                    { (byte)RoomParameterCode.DepositAmount, player.CurrentCredit},

                };
                var request = new Request((byte)ServerOperationCode.Deposit, null, param);

                this.SendNotification(RoomNotification.SEND_TO_PROXY, request);
            }
            // texasUserModel.Remove(userData.UserKey);
            //var texasUser = texasUserModel.User(userData.UserKey);
            // texasUser.isDisconnected = true;
            base.Execute(notification);

        }

        public void PushSitDownNotification(TexasSitDown texasSitDown)
        {
            //var roomModel = this.Facade.RetrieveProxy(RoomModel.NAME) as RoomModel;
            var param = new Dictionary<byte, object>()
            {

                //{ (byte)RoomParameterCode.RoomID, roomModel.RoomData.RoomID },
                { (byte)ParameterCode.TexasSitDown, JsonConvert.SerializeObject(texasSitDown) },
            };

            var eevent = new Event((byte)TexasEventCode.SitDown, null, param);

            this.SendNotification(RoomNotification.SEND_EVENT_TO_ALL_USER, eevent);
        }
    }
}
