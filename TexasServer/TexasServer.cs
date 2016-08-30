using Autofac;
using System.Collections.Generic;
using VuiLen.TexasServer.Game;
using VuiLenGameFramework.Room.Config;
using VuiLenServerFramework;

namespace VuiLen.TexasServer
{
    public class Application : PhotonApplication 
    {

        protected override void Setup()
        {
            base.Setup();

            var defaultRooms = Container.Resolve<IEnumerable<GameDefaultRoomConfig>>();
            
            foreach (var roomItem in defaultRooms)
            {
                TexasApplication.CreateRoom(ServerApplication,roomItem.Room);
                Log.InfoFormat("Default Room" + roomItem.Room.RoomID);

            }
        }
    }
}
