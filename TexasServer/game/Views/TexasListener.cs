using System.Collections.Generic;
using TexasServerCommon.Codes;
using VuiLen.Texas.Game.StateMachine;
using VuiLenGameFramework;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;

namespace VuiLen.Texas.Game.Views
{
    public class TexasListener : Mediator
    {
        new public const string NAME = "TexasListener";

        public override IEnumerable<string> ListNotificationInterests
        {
            get
            {

                return new List<string>(new string[] {
                    GameApplicationNotification.SEND_TO_FACADE
                });

            }
        }

        public override void HandleNotification(INotification notification)
        {
            var stateMachine = this.Facade.RetrieveMediator(TexasStateMachine.NAME) as TexasStateMachine;
            Log.DebugFormat("TEXAS START STATE MACHINE CALLED FROM LISTENER");

            switch (stateMachine.States.State)
            {
                 
                case StateCode.WaitingNewGame:
                    stateMachine.States.Fire(TexasTrigger.state_waiting_new_game);
                    break;
                case StateCode.StartGame:
                    Log.DebugFormat("TEXAS START GAME CALLED FROM LISTENER");
                    stateMachine.States.Fire(TexasTrigger.state_start_game);
                    break;
                case StateCode.PreFlop:
                    stateMachine.States.Fire(TexasTrigger.state_pre_flop);
                    break;
                case StateCode.Flop:
                    stateMachine.States.Fire(TexasTrigger.state_flop);
                    break;
                case StateCode.Turn:
                    stateMachine.States.Fire(TexasTrigger.state_turn);
                    break;
                case StateCode.River:
                    stateMachine.States.Fire(TexasTrigger.state_river);
                    break;
                case StateCode.ShowDown:
                    stateMachine.States.Fire(TexasTrigger.state_showdown);
                    break;
                case StateCode.FlipCard:
                    stateMachine.States.Fire(TexasTrigger.state_flip_card);
                    break;
                case StateCode.Calculating:
                    stateMachine.States.Fire(TexasTrigger.state_calculating);
                    break;
                case StateCode.Done:
                    stateMachine.States.Fire(TexasTrigger.state_done);
                    break;
            }
        }
    }
}
