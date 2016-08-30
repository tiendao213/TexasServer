

using VuiLen.Texas.Game.Models;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;
using VuiLenGameFramework.Room.Models;
using VuiLenServerCommon.MessageObjects;
using VuiLenServerFramework;

namespace VuiLen.Texas.Game.Controllers.Initializes
{
    public class TexasPrepareModelCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            Log.DebugFormat("TexasPrepareModelCommand");

            var server = notification.Body.GetType().GetProperty("server").GetValue(notification.Body, null) as IServerApplication;
            var roomItem = notification.Body.GetType().GetProperty("roomItem").GetValue(notification.Body, null) as RoomItem;


            Facade.RegisterProxy(new ServerModel(server));
            Facade.RegisterProxy(new RoomModel(roomItem));
            Facade.RegisterProxy(new RoomUserModel());

            Facade.RegisterProxy(new TexasModel());
        }
    }
}
