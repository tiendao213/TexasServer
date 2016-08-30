using ExitGames.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using VuiLen.TexasServer.Game;
using VuiLenGameFramework;
using VuiLenGameFramework.Handlers;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework;
using VuiLenServerFramework.Implementation.Messaging;
using VuiLenServerFramework.Interfaces.Messaging;
using VuiLenServerFramework.Interfaces.Server;

namespace VuiLen.TexasServer.Handlers
{
    public class CreateRoomTexasHandler : CreateRoomHandler
    {
        public IServerApplication ServerApplication { get; set; }
        public CreateRoomTexasHandler(
               ILogger log,
               IServerApplication serverApplication
           ) : base(log)
        {
            ServerApplication = serverApplication;
        }

        public override bool OnHandleMessage(IMessage message, IServerPeer serverPeer)
        {
            if (base.OnHandleMessage(message, serverPeer))
            {
                Log.InfoFormat("CreateRoomTexasHandler :: " + roomItem.Name);

                //create room handle in application
                if (TexasApplication.CreateRoom(ServerApplication, roomItem))
                {

                    var param = new Dictionary<byte, object>() {
                        { (byte)ServerParameterCode.PeerId, message.Parameters[(byte)ServerParameterCode.PeerId] },
                        { (byte)ClientParameterCode.SubOperationCode,message.Parameters[(byte)ClientParameterCode.SubOperationCode]},
                        { (byte)RoomParameterCode.Item, JsonConvert.SerializeObject(roomItem)},
                    };
                    var responseMessage = new Response((byte)RoomOperationCode.Create, null, param, "Room Create Success", (byte)ErrorCode.OK);
                    serverPeer.SendMessage(responseMessage);
                }
            }

            return true;
        }
    }
}
