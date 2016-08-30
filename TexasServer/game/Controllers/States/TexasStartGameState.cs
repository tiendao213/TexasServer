

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;
using VuiLen.Texas.Game.Models;
using VuiLen.Texas.Game.StateMachine;
using VuiLen.TexasServer.game.Controllers.Room;
using VuiLen.TexasServer.game.Models.vos;
using VuiLenGameFramework;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;
using VuiLenGameFramework.Room.Models;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework.Implementation.Messaging;

namespace VuiLen.Texas.Game.Controllers.States
{
    public class TexasStartGameState : SimpleCommand, TexasSitDownListener
    {
        public override void Execute(INotification notification)
        {
            // Log.DebugFormat("TEXAS STATE(START GAME) CALLED");
            //var stateMachine = this.Facade.RetrieveMediator(TexasStateMachine.NAME) as TexasStateMachine;
            //stateMachine.Fire(3000);
            Log.DebugFormat("STATE(START GAME) HANDLE");

            var texasModel = this.Facade.RetrieveProxy(TexasModel.NAME) as TexasModel;

            if (texasModel.isPlaying)
            {
                return;
            }


            // validate game credit
            // if current credit < big blind force unsit
            texasModel.forceUnsitIfNotEnoughtMoney(this);
            if (!texasModel.isEnoughtPlayer())
            {
                return;
            }
            Log.DebugFormat("VALIDATE DONE READY START GAME");
            texasModel.startGame();

            sendStartGame(texasModel);
            

            // sendMinBet(texasModel);

        }

        

        private void sendMinBet(TexasModel texasModel)
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

                // send update
                var param = new Dictionary<byte, object>()
                {
                    { (byte)ClientParameterCode.UserKey, slot.UserKey },
                    { (byte)ParameterCode.TexasPlayerItem, JsonConvert.SerializeObject(slot.build()) },
                };

                var eevent = new Event((byte)TexasEventCode.UpdatePlayerItem, null, param);
                this.SendNotification(RoomNotification.SEND_EVENT_TO_USER, eevent);

                // send min bet
                param = new Dictionary<byte, object>()
                {
                    { (byte)ClientParameterCode.UserKey, slot.UserKey },
                    { (byte)ParameterCode.TexasMinBet, minBets[index] },
                };

                eevent = new Event((byte)TexasEventCode.UpdateMinBet, null, param);
                this.SendNotification(RoomNotification.SEND_EVENT_TO_USER, eevent);
            }
        }

        private void sendStartGame(TexasModel texasModel)
        {
            Log.DebugFormat("SEND START GAME");
            // send start game
            TexasStartGame texasStartgame = new TexasStartGame();
            texasStartgame.BigBlindIndex = texasModel.BigBlindIndex;
            texasStartgame.DealderIndex = texasModel.DealerIndex;
            texasStartgame.SmallBlindIndex = texasModel.SmallBlindIndex;
            texasStartgame.TableState = texasModel.TableState;

            var param2 = new Dictionary<byte, object>()
                {
                    { (byte)ParameterCode.TexasStart, JsonConvert.SerializeObject(texasStartgame) },
                };

            var eevent2 = new Event((byte)TexasEventCode.StartGame, null, param2);
            this.SendNotification(RoomNotification.SEND_EVENT_TO_ALL_USER, eevent2);
        }

        public void PushSitDownNotification(TexasSitDown texasSitDown)
        {
            // var roomModel = this.Facade.RetrieveProxy(RoomModel.NAME) as RoomModel;
            var param = new Dictionary<byte, object>()
            {

                // { (byte)RoomParameterCode.RoomID, roomModel.RoomData.RoomID },
                { (byte)ParameterCode.TexasSitDown, JsonConvert.SerializeObject(texasSitDown) },
            };

            var eevent = new Event((byte)TexasEventCode.SitDown, null, param);

            this.SendNotification(RoomNotification.SEND_EVENT_TO_ALL_USER, eevent);
        }
    }
}
