
using TexasServerCommon.MessageObjects;

namespace VuiLen.TexasServer.game.Controllers.Room
{
    public interface TexasSitDownListener
    {
        void PushSitDownNotification(TexasSitDown texasSitDown);
    }
}
