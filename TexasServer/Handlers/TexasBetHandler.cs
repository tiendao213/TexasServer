
using ExitGames.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;
using VuiLen.SicBoServer.Operations;
using VuiLen.Texas.Game.StateMachine;
using VuiLen.TexasServer.Game;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework.Implementation.Messaging;
using VuiLenServerFramework.Implementation.Server;
using VuiLenServerFramework.Interfaces.Client;
using VuiLenServerFramework.Interfaces.Messaging;
using VuiLenServerFramework.Interfaces.Server;

namespace VuiLen.TexasServer.Handlers
{
    public class TexasBetHandler : ServerHandler
    {
        public ILogger Log;
        private IClientDataList _userList;

        public TexasBetHandler(ILogger log, IClientDataList userList)
        {
            Log = log;
            _userList = userList;
        }

        public override byte Code
        {
            get
            {
                return (byte)TexasOperationCode.Bet;
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
            Log.DebugFormat("OnHandleMessage:TexasBetHandler");

            var errorParam = new Dictionary<byte, object>
            {
                { (byte)ServerParameterCode.PeerId, message.Parameters[(byte)ServerParameterCode.PeerId] },
                { (byte)ClientParameterCode.SubOperationCode,message.Parameters[(byte)ClientParameterCode.SubOperationCode]},
            };

            var serverPeer = sPeer as ServerOutbouncePeer;
            var operation = new BetOperator(serverPeer.Protocol, message);

            if (!operation.IsValid)
            {
                serverPeer.SendMessage(new Response((byte)TexasOperationCode.Bet, null, errorParam, operation.GetErrorMessage(), (int)RoomErrorCode.OperationInvalid));
                return false;
            }
            var peerId = new Guid((Byte[])operation.PeerId);

            var roomID = operation.RoomID;
            errorParam.Add((byte)RoomParameterCode.RoomID, roomID);
            var facade = TexasApplication.GetRoom(roomID);
            if (facade == null)
            {
                serverPeer.SendMessage(new Response((byte)TexasOperationCode.Bet, null, errorParam, "RoomId Not Found", (int)RoomErrorCode.RoomIdNotFound));
                return false;

            }
            var userItem = TexasApplication.GetRoomUser(roomID, peerId);

            if (userItem == null)
            {
                serverPeer.SendMessage(new Response((byte)TexasOperationCode.Bet, null, errorParam, "User Not Yet Join Room", (int)RoomErrorCode.UserNotYetJoinRoom));
                return false;
            }

            TexasBetRequest texasBetRequest = JsonConvert.DeserializeObject<TexasBetRequest>(operation.TexasBetRequest);
            texasBetRequest.UserKey = userItem.UserKey;

            var stateMachine = facade.RetrieveMediator(TexasStateMachine.NAME) as TexasStateMachine;
            stateMachine.Bet(texasBetRequest);
            return true;
        }


    }
}
