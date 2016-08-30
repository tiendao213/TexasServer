
using System;
using ExitGames.Logging;
using VuiLenServerFramework.Implementation.Messaging;
using VuiLenServerFramework.Interfaces.Client;
using VuiLenServerFramework.Interfaces.Messaging;
using VuiLenServerFramework.Interfaces.Server;
using TexasServerCommon.Codes;
using VuiLen.TexasServer.Operations;
using VuiLenServerFramework.Implementation.Server;
using System.Collections.Generic;
using VuiLenServerCommon.Codes;
using VuiLen.TexasServer.Game;
using TexasServerCommon.MessageObjects;
using Newtonsoft.Json;
using VuiLen.Texas.Game.StateMachine;

namespace VuiLen.TexasServer.Handlers
{
    public class TexasSitHandler : ServerHandler
    {
        public ILogger Log;
        private IClientDataList _userList;

        public TexasSitHandler(ILogger log, IClientDataList userList)
        {
            Log = log;
            _userList = userList;
        }

        public override byte Code
        {
            get
            {
                return (byte) TexasOperationCode.Sit;
            }
        }

        public override int? SubCode
        {
            get
            {
                return null;
            }
        }

        public override MessageType Type
        {
            get { return MessageType.Async | MessageType.Request | MessageType.Response; }
        }

        public override bool OnHandleMessage(IMessage message, IServerPeer sPeer)
        {
            Log.DebugFormat("OnHandleMessage:TexasSitHandler");

            var errorParam = new Dictionary<byte, object>
            {
                { (byte)ServerParameterCode.PeerId, message.Parameters[(byte)ServerParameterCode.PeerId] },
                { (byte)ClientParameterCode.SubOperationCode,message.Parameters[(byte)ClientParameterCode.SubOperationCode]},
            };

            var serverPeer = sPeer as ServerOutbouncePeer;
            var operation = new SitOperator(serverPeer.Protocol, message);

            if (!operation.IsValid)
            {
                serverPeer.SendMessage(new Response((byte)TexasOperationCode.Sit, null, errorParam, operation.GetErrorMessage(), (int)RoomErrorCode.OperationInvalid));
                return false;
            }
            var peerId = new Guid((Byte[])operation.PeerId);

            var roomID = operation.RoomID;
            errorParam.Add((byte)RoomParameterCode.RoomID, roomID);

            var facade = TexasApplication.GetRoom(roomID);
            if (facade == null)
            {
                serverPeer.SendMessage(new Response((byte)TexasOperationCode.Sit, null, errorParam, "RoomId Not Found", (int)RoomErrorCode.RoomIdNotFound));
                return false;

            }
            var userItem = TexasApplication.GetRoomUser(roomID, peerId);

            if (userItem == null)
            {
                serverPeer.SendMessage(new Response((byte)TexasOperationCode.Sit, null, errorParam, "User Not Yet Join Room", (int)RoomErrorCode.UserNotYetJoinRoom));
                return false;
            }
           
            var json = operation.TexasSitDown;
            TexasSitDown texasSitDown = JsonConvert.DeserializeObject<TexasSitDown>(json) as TexasSitDown;
            texasSitDown.sisDownInfo.UserKey = userItem.UserKey;
            texasSitDown.sisDownInfo.UserName = userItem.UserName;

            var texasRoomItem = TexasApplication.GetRoomItem(roomID);
            if (userItem.BankBalance < texasRoomItem.WithdrawMin && texasSitDown.sisDownInfo.IsSitDown)
            {
                serverPeer.SendMessage(new Response((byte)TexasOperationCode.Sit, null, errorParam, "Need $" + texasRoomItem.WithdrawMin + " to play", (int)RoomErrorCode.AmountTooLow));
                return false;
            }

            var stateMachine = facade.RetrieveMediator(TexasStateMachine.NAME) as TexasStateMachine;
            stateMachine.Sit(texasSitDown);

            return true;
        }

        
    }
}
