
using Newtonsoft.Json;
using System.Collections.Generic;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;
using VuiLen.Texas.Game.Models;
using VuiLen.TexasServer.game.Controllers.Room;
using VuiLen.TexasServer.game.Models.vos;
using VuiLenGameFramework;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;
using VuiLenServerFramework.Implementation.Messaging;

namespace VuiLen.Texas.Game.Controllers.States.Actions
{
    public class TexasUpdateSlotCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            Slot slot = notification.Body as Slot;
            PlayerItem playerItem = slot.build();

            var param = new Dictionary<byte, object>()
            {
                { (byte)ParameterCode.TexasPlayerItem, JsonConvert.SerializeObject(playerItem) },
            };

            var eevent = new Event((byte)TexasEventCode.UpdatePlayerItem, null, param);

            this.SendNotification(RoomNotification.SEND_EVENT_TO_ALL_USER, eevent);
        }

    }
}
