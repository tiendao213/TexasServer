
using VuiLen.Texas.Game.StateMachine;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;

namespace VuiLen.Texas.Game.Controllers.States
{
    public class TexasFlipCardState : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            Log.DebugFormat("TEXAS STATE(FLIP CARD) CALLED");
            var stateMachine = this.Facade.RetrieveMediator(TexasStateMachine.NAME) as TexasStateMachine;
            stateMachine.Fire(3000);
        }
    }
}
