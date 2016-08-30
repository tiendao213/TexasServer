using ExitGames.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;
using VuiLen.Texas.Game.Models;
using VuiLen.Texas.Game.StateMachine;
using VuiLen.TexasServer.game.Models.vos;
using VuiLenGameFramework;
using VuiLenServerCommon.Codes;
using VuiLenServerCommon.MessageObjects;
using VuiLenServerFramework.Implementation.Messaging;
using VuiLenServerFramework.Implementation.Server;
using VuiLenServerFramework.Interfaces.Messaging;
using VuiLenServerFramework.Interfaces.Server;

namespace VuiLen.TexasServer.Handlers
{
    public class TexasBuyinSuccessHandler : ServerHandler
    {

        public ILogger Log;

        protected RoomItem roomItem { get; set; }

        public TexasBuyinSuccessHandler(
               ILogger log
           )
        {
            Log = log;
        }

        public override byte Code
        {
            get { return (byte)ServerOperationCode.WithdrawSuccess; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        public override MessageType Type
        {
            get { return MessageType.Async | MessageType.Request | MessageType.Response; }
        }

        public override bool OnHandleMessage(IMessage message, IServerPeer sPeer)
        {

            Log.DebugFormat("TexasBuyinSuccessHandler");

            var serverPeer = sPeer as ServerOutbouncePeer;

            var operation = new BuyinSuccessOperator(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                return false;
            }

            var roomId = operation.RoomID;
            var peerId = new Guid((Byte[])operation.PeerId);
            var amount = operation.Amount;

            var facade = GameApplicationBase.GetRoom(roomId);

            if (facade == null)
            {
                return false;

            }

            var userItem = GameApplicationBase.GetRoomUser(roomId, peerId);

            if (userItem == null)
            {
                return false;
            }

            Log.DebugFormat("TexasBuyinSuccessHandler Good :: " + peerId + " " + userItem.UserKey);

            var userKey = userItem.UserKey;

            var texasModel = facade.RetrieveProxy(TexasModel.NAME) as TexasModel;

            Slot player = texasModel.getSlot(userKey);
            if (player != null)
            {
                player.CurrentCredit += amount;
                Dictionary<byte, object> param = new Dictionary<byte, object>()
                    {
                        { (byte)ServerParameterCode.PeerId, message.Parameters[(byte)ServerParameterCode.PeerId] },
                        { (byte)ServerParameterCode.SubOperationCode,message.Parameters[(byte)ServerParameterCode.SubOperationCode]},
                        { (byte)RoomParameterCode.RoomID, roomId},
                        { (byte)RoomParameterCode.WithdrawAmount,  amount },
                        { (byte)RoomParameterCode.GameCredit, player.CurrentCredit },

                        { (byte)RoomParameterCode.RoomUser, JsonConvert.SerializeObject(userItem)}
                    };
                serverPeer.SendMessage(new Response((byte)RoomOperationCode.Withdraw, null, param, "Buyin Success", (int)RoomErrorCode.OK));

                TexasBuyin texasBuyIn = new TexasBuyin();
                texasBuyIn.BuyinInfo = new BuyinInfo();
                texasBuyIn.BuyinInfo.AvatarKey = player.AvatarKey;
                texasBuyIn.BuyinInfo.Credit = player.CurrentCredit;
                texasBuyIn.BuyinInfo.Index = texasModel.getSlotIndex(player);
                texasBuyIn.BuyinInfo.UserKey = player.UserKey;
                texasBuyIn.BuyinInfo.UserName = player.UserName;

                param = new Dictionary<byte, object>()
                {
                    { (byte)ParameterCode.TexasBuyin, JsonConvert.SerializeObject(texasBuyIn) },
                };

                var eevent = new Event((byte)TexasEventCode.BuyIn, null, param);

                facade.SendNotification(RoomNotification.SEND_EVENT_TO_ALL_USER, eevent);
            }
            return true;
        }
    }
}
