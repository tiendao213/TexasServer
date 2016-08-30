

using ExitGames.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TexasServerCommon.Codes;
using VuiLen.Texas.Game.Models;
using VuiLen.TexasServer.game.Models.vos;
using VuiLen.TexasServer.Game;
using VuiLen.TexasServer.Operations;
using VuiLenGameFramework;
using VuiLenGameFramework.Handlers;
using VuiLenServerCommon.Codes;
using VuiLenServerCommon.MessageObjects;
using VuiLenServerFramework.Implementation.Messaging;
using VuiLenServerFramework.Implementation.Server;
using VuiLenServerFramework.Interfaces.Messaging;
using VuiLenServerFramework.Interfaces.Server;

namespace VuiLen.TexasServer.Handlers
{
    public class TexasBuyinRoomHandler : WithdrawRoomHandler
    {
        public TexasBuyinRoomHandler(ILogger log) : base(log)
        {
        }

        public override byte Code
        {
            get { return (byte)RoomOperationCode.Withdraw; }
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
            Log.DebugFormat("TEXASWithdrawRoomHandler  ");
            var serverPeer = sPeer as ServerOutbouncePeer;
            var errorParam = new Dictionary<byte, object>
            {
                { (byte)ServerParameterCode.PeerId, message.Parameters[(byte)ServerParameterCode.PeerId] },
                { (byte)ClientParameterCode.SubOperationCode,message.Parameters[(byte)ClientParameterCode.SubOperationCode]},
            };

            var operation = new BuyinSuccessOperator(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                return false;
            }

            var roomId = operation.RoomID;
            var peerId = new Guid((Byte[])operation.PeerId);
            var amount = operation.Amount;
            errorParam.Add((byte)RoomParameterCode.RoomID, roomId);

            Log.DebugFormat("TexasBuyinSuccessHandler: " + roomId + "|" + peerId + "|" + amount);

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

            if (player == null)
            {
                serverPeer.SendMessage(new Response((byte)RoomOperationCode.Withdraw, null, errorParam, "Player not yet on table", (int)TexasRoomErrorCode.UserNotYetOnTable));
                return false;
            }
            if (player.isOnGame)
            {
                serverPeer.SendMessage(new Response((byte)RoomOperationCode.Withdraw, null, errorParam, "Player is playing", (int)TexasRoomErrorCode.UserPlaying));
                return false;
            }

            var texasRoomItem = TexasApplication.GetRoomItem(roomId);
            if (player.CurrentCredit > 30 * texasRoomItem.WithdrawMax / 100)
            {
                serverPeer.SendMessage(new Response((byte)RoomOperationCode.Withdraw, null, errorParam, "Credit is not yet low to withdraw", (int)TexasRoomErrorCode.CreditNotYetLow));
                return false;
            }
            return base.OnHandleMessage(message, sPeer);

            /*
            Log.DebugFormat("WithdrawRoomHandler  ");


            var serverPeer = sPeer as ServerOutbouncePeer;


            var errorParam = new Dictionary<byte, object>
            {
                { (byte)ServerParameterCode.PeerId, message.Parameters[(byte)ServerParameterCode.PeerId] },
                { (byte)ClientParameterCode.SubOperationCode,message.Parameters[(byte)ClientParameterCode.SubOperationCode]},
            };


            var operation = new WithdrawRoomOperator(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {

                serverPeer.SendMessage(new Response((byte)RoomOperationCode.Withdraw, null, errorParam, operation.GetErrorMessage(), (int)RoomErrorCode.OperationInvalid));
                return false;
            }

            var roomID = operation.RoomID;
            var peerId = new Guid((Byte[])operation.PeerId);
            var amount = operation.Amount;
            var serverCode = message.Parameters[(byte)ServerParameterCode.SubOperationCode];

            errorParam.Add((byte)RoomParameterCode.RoomID, roomID);

            if (amount < 1)
            {
                serverPeer.SendMessage(new Response((byte)RoomOperationCode.Withdraw, null, errorParam, "Amount Withdraw too Low", (int)RoomErrorCode.AmountTooLow));
                return false;
            }


            var facade = GameApplicationBase.GetRoom(roomID);

            if (facade == null)
            {
                serverPeer.SendMessage(new Response((byte)RoomOperationCode.Withdraw, null, errorParam, "RoomId Not Found", (int)RoomErrorCode.RoomIdNotFound));
                return false;

            }

            var userItem = GameApplicationBase.GetRoomUser(roomID, peerId);

            if (userItem == null)
            {
                serverPeer.SendMessage(new Response((byte)RoomOperationCode.Withdraw, null, errorParam, "User Not Yet Join Room", (int)RoomErrorCode.UserNotYetJoinRoom));
                return false;
            }

            Log.DebugFormat("WithdrawRoomHandler Good :: " + peerId);

            var param = new Dictionary<byte, object>
            {
                { (byte)ServerParameterCode.SubOperationCode, serverCode },
                { (byte)ServerParameterCode.PeerId, peerId.ToByteArray() },
                { (byte)RoomParameterCode.RoomID, roomID },
                { (byte)RoomParameterCode.WithdrawAmount, amount },

            };

            //send message back to proxy server
            serverPeer.SendMessage(new Request((byte)ServerOperationCode.Withdraw, null, param));

            return true;
            */
        }
    }
}
