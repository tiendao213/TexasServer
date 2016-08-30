using VuiLen.Texas.Game.Controllers.Room;
using VuiLen.Texas.Game.Controllers.States;
using VuiLen.Texas.Game.Controllers.States.Actions;
using VuiLen.Texas.Game.StateMachine;
using VuiLenGameFramework;
using VuiLenGameFramework.MVC.Interfaces;
using VuiLenGameFramework.MVC.Patterns;
using VuiLenGameFramework.Room.Controllers;
namespace VuiLen.Texas.Game.Controllers.Initializes
{
    public class TexasPrepareControllerCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {

            Log.DebugFormat("TexasPrepareControllerCommand");


            Facade.RegisterCommand(RoomNotification.JOIN_REQUEST, new JoinRoomRequestCommand());
            Facade.RegisterCommand(RoomNotification.JOIN, new TexasJoinRoomCommand());
            Facade.RegisterCommand(RoomNotification.LEAVE, new TexasLeaveRoomCommand());
            Facade.RegisterCommand(RoomNotification.LEAVE_FORCE, new LeaveRoomForceCommand());


            Facade.RegisterCommand(RoomNotification.SEND_TO_ALL_USER, new SendToAllUserInRoom());
            Facade.RegisterCommand(RoomNotification.SEND_EVENT_TO_ALL_USER, new SendEventToAllUserInRoom());
            Facade.RegisterCommand(RoomNotification.SEND_EVENT_TO_USER, new SendEventToRemoteUser());
            Facade.RegisterCommand(RoomNotification.SEND_TO_USER, new SendToRemoteUser());
            Facade.RegisterCommand(RoomNotification.SEND_TO_PROXY, new SendToProxyServer());

            // Facade.RegisterCommand( GameApplicationNotification.Test ,new TestCommand() );


            Facade.RegisterCommand(TexasTrigger.action_bet.ToString("G"), new TexasBetCommand());
            Facade.RegisterCommand(TexasTrigger.state_calculating.ToString("G"), new TexasCalculatingState());
            Facade.RegisterCommand(TexasTrigger.state_done.ToString("G"), new TexasDoneState());
            Facade.RegisterCommand(TexasTrigger.state_flip_card.ToString("G"), new TexasFlipCardState());
            Facade.RegisterCommand(TexasTrigger.state_flop.ToString("G"), new TexasFlopState());
            Facade.RegisterCommand(TexasTrigger.state_pre_flop.ToString("G"), new TexasPreFlopState());
            Facade.RegisterCommand(TexasTrigger.state_turn.ToString("G"), new TexasTurnState());
            Facade.RegisterCommand(TexasTrigger.state_river.ToString("G"), new TexasRiverState());
            Facade.RegisterCommand(TexasTrigger.state_showdown.ToString("G"), new TexasShowDownState());
            Facade.RegisterCommand(TexasTrigger.state_start_game.ToString("G"), new TexasStartGameState());
            Facade.RegisterCommand(TexasTrigger.state_waiting_new_game.ToString("G"), new TexasWaitingNewGameState());

            Facade.RegisterCommand(TexasTrigger.action_sit.ToString("G"), new TexasSitCommand());
            Facade.RegisterCommand(TexasTrigger.action_update_slot.ToString("G"), new TexasUpdateSlotCommand());

            Facade.RegisterCommand(TexasTrigger.state_play.ToString("G"), new TexasPlayState());
        }
    }
}
