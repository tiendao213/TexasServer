using ExitGames.Logging;
using System;
using VuiLen.Texas.Game.Controllers.Initializes;
using VuiLenGameFramework;
using VuiLenServerCommon.MessageObjects;
using VuiLenServerFramework;

namespace VuiLen.TexasServer.Game
{
    public class TexasApplication : GameApplicationBase
    {

        public TexasApplication(string roomID,  ILogger log) : base(roomID, log)
        {
        }

        public static bool CreateRoom( IServerApplication serverApplication, RoomItem roomItem )
        {
            //var roomID = roomItem.ID.ToString();
            for (int i = 1; i < 1000; i++)
            {
                var roomID = "Texas" + GameApplicationBase.ID++;
                if (!HasCore(roomID))
                {
                    var newRoom = new TexasApplication(roomID, serverApplication.Log) as TexasApplication;
                    roomItem.Name = roomItem.Name!=""?roomItem.Name: roomID;
                    roomItem.RoomID = roomID;
                    roomItem.Created = DateTime.Now;

                    if (roomItem.Level >= 4)
                    {
                        roomItem.Level = 4;
                        roomItem.WithdrawMin = 1000;
                        roomItem.WithdrawMax = 0;
                    }
                    else if (roomItem.Level >= 3)
                    {
                        roomItem.Level = 3;
                        roomItem.WithdrawMin = 1000;
                        roomItem.WithdrawMax = 0;
                    }
                    else if (roomItem.Level >= 2)
                    {
                        roomItem.Level = 2;
                        roomItem.WithdrawMin = 1000;
                        roomItem.WithdrawMax = 100000;
                    }
                    else 
                    {
                        roomItem.Level = 1;
                        roomItem.WithdrawMin = 1000;
                        roomItem.WithdrawMax = 10000;
                    }
                    m_instanceMap[roomID] = newRoom;
                    newRoom.Startup(new { server = serverApplication, roomItem = roomItem });
                    return true;
                }
            }

            return false;
        }

        protected override void InitializeController()
        {
            base.InitializeController();

            RegisterCommand(GameApplicationNotification.APP_STARTUP,typeof(TexasStartupCommand));
        }

        public void test()
        {
            Log.InfoFormat("taixiu facade::");
            //ServerApplication.test();
        }
    }
}
