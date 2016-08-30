
using VuiLenGameFramework.MVC.Patterns;

namespace VuiLen.Texas.Game.Controllers.Initializes
{
    public class TexasStartupCommand : MacroCommand
    {
        protected override void InitializeMacroCommand()
        {
            this.AddSubCommand(typeof(TexasPrepareModelCommand));

            this.AddSubCommand(typeof(TexasPrepareViewCommand));
            this.AddSubCommand(typeof(TexasPrepareControllerCommand));
        }
    }
}
