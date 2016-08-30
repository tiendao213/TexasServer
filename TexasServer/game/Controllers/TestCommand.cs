using VuiLen.Texas.Game.StateMachine;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;
using VuiLenGameFramework.Room.Models;

namespace VuiLen.TexasServer.Game.Controllers
{
    public class TestCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            //Log.DebugFormat("TestCommand");

            var stateMachine = this.Facade.RetrieveMediator(TexasStateMachine.NAME) as TexasStateMachine;
            //var betTrigger = stateMachine.States.SetTriggerParameters<string>(TexasTrigger.action_bet);
            // stateMachine.Fire(TexasTrigger.state_deal);
            // stateMachine.States.Fire(betTrigger, "taxiu");
            //stateMachine.Bet("Taixiu bet");
           // stateMachine.Move("Taixiu Move");
        }

    }
}
