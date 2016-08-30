

using VuiLen.Texas.Game.Models;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;

namespace VuiLen.Texas.Game.Controllers.States
{
    public class TexasWaitingNewGameState : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            Log.DebugFormat("STATE(WAITING NEW GAME) HANDLE");
            // Log.DebugFormat("TEXAS STATE(WAITING NEW GAME) CALLED");
            var texasModel = this.Facade.RetrieveProxy(TexasModel.NAME) as TexasModel;
            texasModel.reset();
        }
    }
}
