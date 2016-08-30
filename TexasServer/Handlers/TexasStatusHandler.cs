

using ExitGames.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TexasServerCommon.Codes;
using VuiLen.Texas.Game.Models;
using VuiLen.TexasServer.Game;
using VuiLen.TexasServer.Operations;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework.Implementation.Messaging;
using VuiLenServerFramework.Implementation.Server;
using VuiLenServerFramework.Interfaces.Client;
using VuiLenServerFramework.Interfaces.Messaging;
using VuiLenServerFramework.Interfaces.Server;

namespace VuiLen.TexasServer.Handlers
{
    public class TexasStatusHandler : ServerHandler
    {
        public ILogger Log;
        private IClientDataList _userList;

        public TexasStatusHandler(ILogger log, IClientDataList userList)
        {
            Log = log;
            _userList = userList;
        }

        public override byte Code
        {
            get
            {
               return (byte) TexasOperationCode.Status;
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
            Log.DebugFormat("OnHandleMessage:TexasStatusHandler");

            var errorParam = new Dictionary<byte, object>
            {
                { (byte)ServerParameterCode.PeerId, message.Parameters[(byte)ServerParameterCode.PeerId] },
                { (byte)ClientParameterCode.SubOperationCode,message.Parameters[(byte)ClientParameterCode.SubOperationCode]},
            };

            var serverPeer = sPeer as ServerOutbouncePeer;
            var operation = new StatusOperator(serverPeer.Protocol, message);


            if (!operation.IsValid)
            {
                serverPeer.SendMessage(new Response((byte)TexasOperationCode.Status, null, errorParam, operation.GetErrorMessage(), (int)RoomErrorCode.OperationInvalid));
                return false;
            }
            var peerId = new Guid((Byte[])operation.PeerId);

            var roomID = operation.RoomID;
            errorParam.Add((byte)RoomParameterCode.RoomID, roomID);
            var facade = TexasApplication.GetRoom(roomID);
            if (facade == null)
            {
                serverPeer.SendMessage(new Response((byte)TexasOperationCode.Status, null, errorParam, "RoomId Not Found", (int)RoomErrorCode.RoomIdNotFound));
                return false;

            }
            var userItem = TexasApplication.GetRoomUser(roomID, peerId);

            if (userItem == null)
            {
                serverPeer.SendMessage(new Response((byte)TexasOperationCode.Status, null, errorParam, "User Not Yet Join Room", (int)RoomErrorCode.UserNotYetJoinRoom));
                return false;
            }

            var texasModel = facade.RetrieveProxy(TexasModel.NAME) as TexasModel;
            var texasStatus = texasModel.buildTexasHoldemStatus();


            var param = new Dictionary<byte, object>
            {
                { (byte)ServerParameterCode.PeerId, message.Parameters[(byte)ServerParameterCode.PeerId] },
                { (byte)ClientParameterCode.SubOperationCode,message.Parameters[(byte)ClientParameterCode.SubOperationCode]},
                { (byte)RoomParameterCode.RoomID, roomID },
                { (byte)ParameterCode.TexasStatus, JsonConvert.SerializeObject(texasStatus) },

            };

            serverPeer.SendMessage(new Response((byte)TexasOperationCode.Status, null, param, "Texas Status Success", (int)RoomErrorCode.OK));

            return true;
        }
    }
}
