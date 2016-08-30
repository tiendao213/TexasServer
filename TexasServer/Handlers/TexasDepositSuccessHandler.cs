using ExitGames.Logging;
using System.Collections.Generic;
using VuiLenServerFramework.Implementation.Messaging;
using VuiLenServerFramework.Implementation.Server;
using VuiLenServerFramework.Interfaces.Messaging;
using VuiLenServerFramework.Interfaces.Server;
using System;
using VuiLenServerCommon.Codes;
using VuiLenServerCommon.MessageObjects;
using VuiLen.TexasServer.Operations;
using VuiLenGameFramework;
using Newtonsoft.Json;
using VuiLen.Texas.Game.Models;
using VuiLen.TexasServer.game.Models.vos;
using VuiLen.Texas.Game.StateMachine;
using TexasServerCommon.Codes;

namespace VuiLen.TexasServer.Handlers
{
    public class TexasDepositSuccessHandler : ServerHandler
    {

        public ILogger Log;

        protected RoomItem roomItem { get; set; }

        public TexasDepositSuccessHandler(
               ILogger log
           )
        {
            Log = log;
        }

        public override byte Code
        {
            get { return (byte)ServerOperationCode.DepositSuccess; }
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

            Log.DebugFormat("TexasDepositSuccessHandler");


            var serverPeer = sPeer as ServerOutbouncePeer;

            var operation = new DepositSuccessOperator(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {

                Log.DebugFormat("TexasDepositSuccessHandler no Good :: " + operation.GetErrorMessage());


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
                Log.DebugFormat("TexasDepositSuccessHandler user not found ");
                return false;
            }

            Log.DebugFormat("TexasDepositSuccessHandler Good :: " + peerId + " " + userItem.UserKey);

            var userKey = userItem.UserKey;

            var texasModel = facade.RetrieveProxy(TexasModel.NAME) as TexasModel;

            Slot player = texasModel.getSlot(userKey);
            if (player != null)
            {
                player.CurrentCredit -= amount;
            }

            var param = new Dictionary<byte, object>()
            {
                { (byte)ServerParameterCode.PeerId, message.Parameters[(byte)ServerParameterCode.PeerId] },
                { (byte)ServerParameterCode.SubOperationCode,message.Parameters[(byte)ServerParameterCode.SubOperationCode]},
                { (byte)RoomParameterCode.RoomID, roomId},
                { (byte)RoomParameterCode.DepositAmount,  amount },
                { (byte)RoomParameterCode.GameCredit,   player.CurrentCredit },

                { (byte)RoomParameterCode.RoomUser, JsonConvert.SerializeObject(userItem)}
            };
            serverPeer.SendMessage(new Response((byte)RoomOperationCode.Deposit, null, param, "Deposit Success", (int)RoomErrorCode.OK));
            return true;
        }
    }
}
