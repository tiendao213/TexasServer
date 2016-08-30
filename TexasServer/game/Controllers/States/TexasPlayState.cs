using Newtonsoft.Json;
using System.Collections.Generic;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;
using VuiLen.Texas.Game.Models;
using VuiLen.Texas.Game.StateMachine;
using VuiLenGameFramework;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;
using VuiLenServerFramework.Implementation.Messaging;
using System;
using VuiLenServerCommon.Codes;
using VuiLen.TexasServer.game.Models.vos;

namespace VuiLen.Texas.Game.Controllers.States
{
    public class TexasPlayState : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            // called every state next
            Log.DebugFormat("TEXAS STATE(PLAY) HANDLER: ");

            var texasModel = this.Facade.RetrieveProxy(TexasModel.NAME) as TexasModel;

            if (!texasModel.isPlaying)
            {
                return;
            }

            // send table info
            sendTexasGameInfo(texasModel);
            // send slot
            sendSlots(texasModel);

            sendPrivateCard(texasModel);

            // send min bet
            sendMinBets(texasModel);

            sendThinkingPlayer(texasModel);
        }

        private void sendPrivateCard(TexasModel texasModel)
        {
            Log.DebugFormat("SEND PRIVATE CARD");

            var slots = texasModel.getSlots();
            for (int index = 0; index < slots.Length; index++)
            {
                Slot slot = slots[index];
                if (slot.isEmpty())
                {
                    continue;
                }
                //Log.DebugFormat("CARDS ON SLOT: " + slot.Cards);
                // send min bet
                var param = new Dictionary<byte, object>()
                {
                    { (byte)ClientParameterCode.UserKey, slot.UserKey },
                    { (byte)ParameterCode.PrivateCard, JsonConvert.SerializeObject(slot.buildPrivateCard()) }
                };

                var eevent = new Event((byte)TexasEventCode.PrivateCard, null, param);
                this.SendNotification(RoomNotification.SEND_EVENT_TO_USER, eevent);
            }
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
            if (texasStepInfo.PrevPlayer != null)
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

        private void sendSlots(TexasModel texasModel)
        {
            Log.DebugFormat("SEND SLOT UPDATE");
            var slots = texasModel.getSlots();
            for (int index = 0; index < slots.Length; index++)
            {
                Slot slot = slots[index];
                if (slot.isEmpty())
                {
                    continue;
                }

                // send update
                var param = new Dictionary<byte, object>()
                {
                    { (byte)ParameterCode.TexasPlayerItem, JsonConvert.SerializeObject(slot.build()) },
                };

                var eevent = new Event((byte)TexasEventCode.UpdatePlayerItem, null, param);
                this.SendNotification(RoomNotification.SEND_EVENT_TO_ALL_USER, eevent);
            }
        }

        private void sendTexasGameInfo(TexasModel texasModel)
        {
            Log.DebugFormat("SEND TABLE INFO");
            TexasGameInfo texasGameInfo = texasModel.buildTexasGameInfo();

            var param = new Dictionary<byte, object>()
                {
                    { (byte)ParameterCode.TexasGame, JsonConvert.SerializeObject(texasGameInfo) },
                };

            var eevent = new Event((byte)TexasEventCode.GameInfo, null, param);
            this.SendNotification(RoomNotification.SEND_EVENT_TO_ALL_USER, eevent);
        }
    }
}
