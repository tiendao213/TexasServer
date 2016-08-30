
using Newtonsoft.Json;
using System.Collections.Generic;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;
using VuiLen.Texas.Game.Models;
using VuiLen.TexasServer.game.Controllers.Room;
using VuiLenGameFramework;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;
using VuiLenGameFramework.Room.Models;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework.Implementation.Messaging;

namespace VuiLen.Texas.Game.Controllers.States.Actions
{
    public class TexasSitCommand : SimpleCommand, TexasSitDownListener
    {
        public override void Execute(INotification notification)
        {
            TexasSitDown texasSitDown = notification.Body as TexasSitDown;
            Log.DebugFormat("Execute:TexasSitCommand: " + texasSitDown.sisDownInfo.Index);

            var texasModel = this.Facade.RetrieveProxy(TexasModel.NAME) as TexasModel;

            if (texasSitDown.sisDownInfo.IsSitDown)
            {
                texasModel.sit(texasSitDown.sisDownInfo.UserKey, texasSitDown.sisDownInfo.Index, this);
            } else
            {
                texasModel.unsit(texasSitDown.sisDownInfo.UserKey, this);
            }
        }

        public void PushSitDownNotification(TexasSitDown texasSitDown)
        {
            // var roomModel = this.Facade.RetrieveProxy(RoomModel.NAME) as RoomModel;
            var param = new Dictionary<byte, object>()
            {
                
                // { (byte)RoomParameterCode.RoomID, roomModel.RoomData.RoomID },
                { (byte)ParameterCode.TexasSitDown, JsonConvert.SerializeObject(texasSitDown) },
            };

            var eevent = new Event((byte)TexasEventCode.SitDown, null, param);
            this.SendNotification(RoomNotification.SEND_EVENT_TO_ALL_USER, eevent);
        }
    }
}
