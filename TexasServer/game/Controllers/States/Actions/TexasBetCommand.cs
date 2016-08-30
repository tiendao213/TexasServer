
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;
using VuiLen.Texas.Game.Models;
using VuiLen.TexasServer.game.Models.vos;
using VuiLenGameFramework;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;
using VuiLenGameFramework.Room.Models;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework.Implementation.Messaging;

namespace VuiLen.Texas.Game.Controllers.States.Actions
{
    public class TexasBetCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            TexasBetRequest texasBetRequest = notification.Body as TexasBetRequest;
            Log.DebugFormat("TEXAS BET HANDLE");
            var texasModel = this.Facade.RetrieveProxy(TexasModel.NAME) as TexasModel;
            Slot currentPlayer = texasModel.getCurrentPlayer();
            
            var roomUserModel = this.Facade.RetrieveProxy(RoomUserModel.NAME) as RoomUserModel;
            var clientData = roomUserModel.ClientData(texasBetRequest.UserKey);
            var errorParam = new Dictionary<byte, object>()
                {
                    { (byte)ServerParameterCode.PeerId, clientData.PeerId.ToByteArray() },
                };

            if (currentPlayer == null)
            {
                var response = new Response((byte)TexasOperationCode.Bet, null, errorParam, "Current player not found", (byte)TexasServerCommon.Codes.ErrorCode.Error);
                SendNotification(RoomNotification.SEND_TO_USER, response);
                return;
            }
            if (!currentPlayer.isOnGame)
            {
                var response = new Response((byte)TexasOperationCode.Bet, null, errorParam, "Not in game", (byte)TexasServerCommon.Codes.ErrorCode.NotInGame);
                SendNotification(RoomNotification.SEND_TO_USER, response);
                return;
            }
            if (currentPlayer.Status == (int) PlayerStatusCode.Fold)
            {
                var response = new Response((byte)TexasOperationCode.Bet, null, errorParam, "Can not bet on fold", (byte)TexasServerCommon.Codes.ErrorCode.NotInGame);
                SendNotification(RoomNotification.SEND_TO_USER, response);
                return;
            }
            if (currentPlayer.UserKey != texasBetRequest.UserKey)
            {
                var response = new Response((byte)TexasOperationCode.Bet, null, errorParam, "Not current player", (byte)TexasServerCommon.Codes.ErrorCode.NotCurrentPlayer);
                SendNotification(RoomNotification.SEND_TO_USER, response);
                return;
            }

            texasModel.bet(currentPlayer, texasBetRequest);

            // send min bet
            sendMinBets(texasModel);

            sendThinkingPlayer(texasModel);

        }

        private void sendThinkingPlayer(TexasModel texasModel)
        {
            Log.DebugFormat("SEND THINKING PLAYER");
            Slot currentPlayer = texasModel.getCurrentPlayer();
            if (currentPlayer == null)
            {
                return;
            }
            Slot prevPlayer = texasModel.getPrevPlayer();
            TexasStepInfo texasStepInfo = new TexasStepInfo();
            texasStepInfo.CurrentPlayer = currentPlayer.build();
            if (prevPlayer != null)
            {
                texasStepInfo.PrevPlayer = prevPlayer.build();
            }

            // send update
            var param = new Dictionary<byte, object>()
                {
                    { (byte)ParameterCode.TexasStep, JsonConvert.SerializeObject(texasStepInfo) },
                };

            var eevent = new Event((byte)TexasEventCode.StepInfo, null, param);
            this.SendNotification(RoomNotification.SEND_EVENT_TO_ALL_USER, eevent);
        }

        private void sendMinBets(TexasModel texasModel)
        {
            Log.DebugFormat("SEND UPDATE MIN BET");
            var minBets = texasModel.buildMinBets();

            var slots = texasModel.getSlots();
            for (int index = 0; index < slots.Length; index++)
            {
                Slot slot = slots[index];
                if (slot.isEmpty())
                {
                    continue;
                }

                // send min bet
                var param = new Dictionary<byte, object>()
                {
                    { (byte)ClientParameterCode.UserKey, slot.UserKey },
                    { (byte)ParameterCode.TexasMinBet, minBets[index] },
                };

                var eevent = new Event((byte)TexasEventCode.UpdateMinBet, null, param);
                this.SendNotification(RoomNotification.SEND_EVENT_TO_USER, eevent);
            }
        }
    }
}
