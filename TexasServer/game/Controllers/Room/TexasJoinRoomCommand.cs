
using VuiLen.Texas.Game.Models;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.Room.Controllers;
using VuiLenGameFramework.Room.MessageObjects;
using VuiLenGameFramework.Room.Models;

namespace VuiLen.Texas.Game.Controllers.Room
{
    public class TexasJoinRoomCommand : JoinRoomCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            JoinRoomItem joinRoomItem = notification.Body as JoinRoomItem;

            Log.DebugFormat("JoinTexasRoomCommand :: " + joinRoomItem.PeerId);

            var roomUserModel = Facade.RetrieveProxy(RoomUserModel.NAME) as RoomUserModel;

            var peerId = joinRoomItem.PeerId;

            var userData = roomUserModel.UserData(peerId);
            if (userData != null)
            {
                var texasModel = Facade.RetrieveProxy(TexasModel.NAME) as TexasModel;

                Log.DebugFormat("JoinTexasRoomCommand :: " + userData.UserName);

                texasModel.joinRoom(userData);
            }

        }
    }
}
