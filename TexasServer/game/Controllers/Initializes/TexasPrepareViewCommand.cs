
using VuiLen.Texas.Game.StateMachine;
using VuiLen.Texas.Game.Views;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;

namespace VuiLen.Texas.Game.Controllers.Initializes
{
    class TexasPrepareViewCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            Log.DebugFormat("TexasPrepareViewCommand");

            this.Facade.RegisterMediator(new TexasListener());
            this.Facade.RegisterMediator(new TexasStateMachine());
        }
    }
}
