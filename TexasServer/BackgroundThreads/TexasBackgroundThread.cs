using VuiLenServerFramework.Interfaces.Support;
using ExitGames.Logging;
using VuiLenServerFramework.Interfaces.Server;
using VuiLenServerFramework.Interfaces.Config;
using VuiLenServerFramework;
using VuiLenServerFramework.Interfaces.Client;

namespace VuiLen.TexasServer.BackgroundThreads
{
    public class TexasBackgroundThread : IBackgroundThread
    {
        public ILogger Log { get; }

        private readonly IServerOutbounceCollection<IServerType, IServerPeer> _outbounceCollection;
        private readonly IClientDataList _userList;

        
        public TexasBackgroundThread(
            ILogger log, 
           
            IServerOutbounceCollection<IServerType, IServerPeer> outbounceCollection
            
            )
        {
            Log = log;

            _outbounceCollection = outbounceCollection;
           // _userList = userList;

        }

        public void Run(object threadContext)
        {
            //for (var i = 1; i < 10000000; i++)
           // {
                //var userList = _userList.UserItemList();

                //Log.InfoFormat("TexasBackgroundThread::Run - " + userList.Count + " " );
                //foreach (var user in userList)
                //{
                    //Log.InfoFormat("TexasBackgroundThread::Run - " + user.UserKey + " " + userList.Count + " " + user.BankBalance);
               // }


                //System.Threading.Thread.Sleep(10000);

            //}
        }
        public void Setup(IServerApplication server)
        {
            Log.InfoFormat("TexasBackgroundThread::Setup");

        }

        public void Stop()
        {
            Log.InfoFormat("TexasBackgroundThread::Stop");
        }
    }
}
