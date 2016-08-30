using System;
using Stateless;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;
using VuiLen.Texas.Game.Models;
using VuiLen.TexasServer.game.Models.vos;
using VuiLen.Utilities;
using VuiLenGameFramework.MVC.Patterns;

namespace VuiLen.Texas.Game.StateMachine
{
    public class TexasStateMachine : Mediator
    {

        new public const string NAME = "TexasStateMachine";

        public TexasStateMachine():base(NAME){ }

        private TimerQueue Queue { get; set; }

        public StateMachine<StateCode, TexasTrigger> States { get; set; }

        //private StateMachine<StateCode, TexasTrigger>.TriggerWithParameters<DealItem> _readyTrigger;
        //private StateMachine<StateCode, TexasTrigger>.TriggerWithParameters<TexasBetItem> _betTrigger;
        //private StateMachine<StateCode, TexasTrigger>.TriggerWithParameters<TexasMoveItem> _moveTrigger;
        private StateMachine<StateCode, TexasTrigger>.TriggerWithParameters<Slot> _updateSlotTrigger;
        private StateMachine<StateCode, TexasTrigger>.TriggerWithParameters<TexasSitDown> _sitTrigger;
        private StateMachine<StateCode, TexasTrigger>.TriggerWithParameters<TexasBetRequest> _betTrigger;
        private StateMachine<StateCode, TexasTrigger>.TriggerWithParameters<TexasStartGame> _startGameTrigger;

        public override void OnRegister()
        {
            Log.DebugFormat("TEXAS MACHINE IS ON REGISTERED");
            base.OnRegister();

            Queue = new TimerQueue(20);

            Queue.Initialize();

            States = new StateMachine<StateCode, TexasTrigger>(StateCode.WaitingNewGame);

            _updateSlotTrigger = States.SetTriggerParameters<Slot>(TexasTrigger.action_update_slot);
            _sitTrigger = States.SetTriggerParameters<TexasSitDown>(TexasTrigger.action_sit);
            _betTrigger = States.SetTriggerParameters<TexasBetRequest>(TexasTrigger.action_bet);
            _startGameTrigger = States.SetTriggerParameters<TexasStartGame>(TexasTrigger.state_pre_flop);

            States.Configure(StateCode.WaitingNewGame)
                .OnEntry(stateChanged)
                .PermitReentry(TexasTrigger.action_sit)
                .OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
                .PermitReentry(TexasTrigger.action_update_slot)
                .OnEntryFrom(_updateSlotTrigger, slotPar => OnUpdateSlot(slotPar))
                .Permit(TexasTrigger.state_start_game, StateCode.StartGame)
                // .Permit(TexasTrigger.state_waiting_new_game, StateCode.WaitingNewGame)
                //.OnEntryFrom(_startGameTrigger, startPar => OnStartGame(startPar));
                ;

            States.Configure(StateCode.StartGame)
                // .OnEntry(stateChanged)
                //.OnEntryFrom(_startGameTrigger, startPar => OnStartGame(startPar))
                .PermitReentry(TexasTrigger.action_sit)
                .OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
                .PermitReentry(TexasTrigger.action_update_slot)
                .OnEntryFrom(_updateSlotTrigger, slotPar => OnUpdateSlot(slotPar))
                .Permit(TexasTrigger.state_play, StateCode.Play)
                //.PermitReentry(TexasTrigger.action_sit)
                //.OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
                //.OnEntryFrom(TexasTrigger.state_pre_flop, stateChanged)
                //.Permit(TexasTrigger.state_pre_flop, StateCode.PreFlop);
                ;

            States.Configure(StateCode.Play)
                .PermitReentry(TexasTrigger.action_sit)
                .OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
                .PermitReentry(TexasTrigger.action_update_slot)
                .OnEntryFrom(_updateSlotTrigger, slotPar => OnUpdateSlot(slotPar))
                .PermitReentry(TexasTrigger.action_bet)
                .OnEntryFrom(_betTrigger, betPar => OnBet(betPar))
                //.OnEntry(stateChanged)
                ;

            States.Configure(StateCode.PreFlop)
                .PermitReentry(TexasTrigger.action_sit)
                .OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
                .PermitReentry(TexasTrigger.action_update_slot)
                .OnEntryFrom(_updateSlotTrigger, slotPar => OnUpdateSlot(slotPar))
                .PermitReentry(TexasTrigger.action_bet)
                .OnEntryFrom(_betTrigger, betPar => OnBet(betPar))
                .OnEntry(stateChanged)
                .Permit(TexasTrigger.state_flop, StateCode.Flop);
                

            States.Configure(StateCode.Flop)
                .PermitReentry(TexasTrigger.action_sit)
                .OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
                .PermitReentry(TexasTrigger.action_update_slot)
                .OnEntryFrom(_updateSlotTrigger, slotPar => OnUpdateSlot(slotPar))
                .PermitReentry(TexasTrigger.action_bet)
                .OnEntryFrom(_betTrigger, betPar => OnBet(betPar))
                .OnEntry(stateChanged)
                .Permit(TexasTrigger.state_turn, StateCode.Turn);

            States.Configure(StateCode.Turn)
                .PermitReentry(TexasTrigger.action_sit)
                .OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
                .PermitReentry(TexasTrigger.action_update_slot)
                .OnEntryFrom(_updateSlotTrigger, slotPar => OnUpdateSlot(slotPar))
                .PermitReentry(TexasTrigger.action_bet)
                .OnEntryFrom(_betTrigger, betPar => OnBet(betPar))
                .OnEntry(stateChanged)
                .Permit(TexasTrigger.state_river, StateCode.River);

            States.Configure(StateCode.River)
                .PermitReentry(TexasTrigger.action_sit)
                .OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
                .PermitReentry(TexasTrigger.action_update_slot)
                .OnEntryFrom(_updateSlotTrigger, slotPar => OnUpdateSlot(slotPar))
                .PermitReentry(TexasTrigger.action_bet)
                .OnEntryFrom(_betTrigger, betPar => OnBet(betPar))
                .OnEntry(stateChanged)
                .Permit(TexasTrigger.state_showdown, StateCode.ShowDown);

            States.Configure(StateCode.ShowDown)
                .PermitReentry(TexasTrigger.action_sit)
                .OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
                .PermitReentry(TexasTrigger.action_update_slot)
                .OnEntryFrom(_updateSlotTrigger, slotPar => OnUpdateSlot(slotPar))
                .PermitReentry(TexasTrigger.action_bet)
                .OnEntryFrom(_betTrigger, betPar => OnBet(betPar))
                .OnEntry(stateChanged)
                .Permit(TexasTrigger.state_flip_card, StateCode.FlipCard);

            States.Configure(StateCode.FlipCard)
                .PermitReentry(TexasTrigger.action_sit)
                .OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
                .PermitReentry(TexasTrigger.action_update_slot)
                .OnEntryFrom(_updateSlotTrigger, slotPar => OnUpdateSlot(slotPar))
                .OnEntry(stateChanged)
                .Permit(TexasTrigger.state_calculating, StateCode.Calculating);

            States.Configure(StateCode.Calculating)
               .PermitReentry(TexasTrigger.action_sit)
               .OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
               .PermitReentry(TexasTrigger.action_update_slot)
                .OnEntryFrom(_updateSlotTrigger, slotPar => OnUpdateSlot(slotPar))
               .OnEntry(stateChanged)
               .Permit(TexasTrigger.state_done, StateCode.Done);

            States.Configure(StateCode.Done)
               .PermitReentry(TexasTrigger.action_sit)
               .OnEntryFrom(_sitTrigger, sitPar => OnSit(sitPar))
               .PermitReentry(TexasTrigger.action_update_slot)
                .OnEntryFrom(_updateSlotTrigger, slotPar => OnUpdateSlot(slotPar))
               .OnEntry(stateChanged)
               .Permit(TexasTrigger.state_waiting_new_game, StateCode.WaitingNewGame);

            States.OnUnhandledTrigger((state, trigger) =>
            {

            });

            // States.Fire(TexasTrigger.state_waiting_new_game);
            // WaitingNewGame();
            SendNotification(TexasTrigger.state_waiting_new_game.ToString("G"), null);
        }

        internal void Bet(TexasBetRequest texasBetRequest)
        {
            this.States.Fire(_betTrigger, texasBetRequest);
        }

        private void OnUpdateSlot(Slot slotPar)
        {
            SendNotification(TexasTrigger.action_update_slot.ToString("G"), slotPar);
        }

        internal void UpdateSlot(Slot assignee)
        {
            this.States.Fire(_updateSlotTrigger, assignee);
        }

        private void OnBet(TexasBetRequest texasBetRequest)
        {
            // Log.DebugFormat("TEXAS SIT CALLED: " + this.States.State);
            SendNotification(TexasTrigger.action_bet.ToString("G"), texasBetRequest);
        }

        private void OnSit(TexasSitDown texasSitDown)
        {
            Log.DebugFormat("TEXAS SIT CALLED: " + this.States.State);
            SendNotification(TexasTrigger.action_sit.ToString("G"), texasSitDown);
        }

        
        private void OnStartGame(TexasStartGame texasStartGame)
        {
            Log.DebugFormat("TEXAS ON START GAME: " + this.States.State);
            SendNotification(TexasTrigger.state_start_game.ToString("G"), texasStartGame);
            Log.DebugFormat("READ");
            this.States.Fire(TexasTrigger.state_play);
            
        }

        public void Sit(TexasSitDown assignee)
        {
            // Log.DebugFormat("TEXAS SIT FIRE");
            this.States.Fire(_sitTrigger, assignee);
        }

        public void StartGame(TexasStartGame texasStartGame)
        {
            //
            Log.DebugFormat("TEXAS GAME FIRE START: " + this.States.State);
            SendNotification(TexasTrigger.state_start_game.ToString("G"), texasStartGame);
            
            var texasModel = this.Facade.RetrieveProxy(TexasModel.NAME) as TexasModel;
            if (texasModel.isPlaying && this.States.State == StateCode.StartGame)
            {
                Log.DebugFormat("READY NEXT STATE TO PLAY: " + this.States.State);
                States.Fire(TexasTrigger.state_play);
                SendNotification(TexasTrigger.state_play.ToString("G"), texasStartGame);
            }
            // States.Fire(_startGameTrigger, texasStartGame);
            // 
            // stateChanged();
        }

        /*
        public void WaitingNewGame()
        {
            var texasModel = this.Facade.RetrieveProxy(TexasModel.NAME) as TexasModel;
            texasModel.reset();
            SendNotification(TexasTrigger.state_waiting_new_game.ToString("G"), null);
        }
        */

        
        private void stateChanged()
        {
            Log.DebugFormat("STATE MACHINE(STATE CHANGE): " + this.States.State);
            switch (States.State)
            {
                case StateCode.WaitingNewGame:
                    SendNotification(TexasTrigger.state_waiting_new_game.ToString("G"), null);
                    Queue.SetTimer(timeToFire, null, 500);
                    return;
                case StateCode.StartGame:
                    SendNotification(TexasTrigger.state_start_game.ToString("G"), null);
                    Queue.SetTimer(timeToFire, null, 5000);
                    return;
                case StateCode.Play:
                    SendNotification(TexasTrigger.state_play.ToString("G"), null);
                    //Queue.SetTimer(timeToFire, null, 5000);
                    return;
                case StateCode.PreFlop:
                    SendNotification(TexasTrigger.state_pre_flop.ToString("G"), null);
                    //Queue.SetTimer(timeToFire, null, 5000);
                    return;
                case StateCode.Flop:
                    SendNotification(TexasTrigger.state_flop.ToString("G"), null);
                    //Queue.SetTimer(timeToFire, null, 5000);
                    return;
                case StateCode.Turn:
                    SendNotification(TexasTrigger.state_turn.ToString("G"), null);
                    //Queue.SetTimer(timeToFire, null, 5000);
                    return;
                case StateCode.River:
                    SendNotification(TexasTrigger.state_river.ToString("G"), null);
                    //Queue.SetTimer(timeToFire, null, 5000);
                    return;
                case StateCode.ShowDown:
                    SendNotification(TexasTrigger.state_showdown.ToString("G"), null);
                    //Queue.SetTimer(timeToFire, null, 5000);
                    return;
                case StateCode.FlipCard:
                    SendNotification(TexasTrigger.state_flip_card.ToString("G"), null);
                    //Queue.SetTimer(timeToFire, null, 5000);
                    return;
                case StateCode.Calculating:
                    SendNotification(TexasTrigger.state_calculating.ToString("G"), null);
                    // Queue.SetTimer(timeToFire, null, 5000);
                    return;
                case StateCode.Done:
                    SendNotification(TexasTrigger.state_done.ToString("G"), null);
                    Queue.SetTimer(timeToFire, null, 5000);
                    return;

            }
        }

        private void timeToFire(object state)
        {
            Log.DebugFormat("STATE MACHINE(TIME TO FIRE): " + this.States.State);
            switch (this.States.State)
            {
                case StateCode.WaitingNewGame:
                    this.Fire(TexasTrigger.state_start_game);
                    return;
                case StateCode.StartGame:
                    this.Fire(TexasTrigger.state_pre_flop);
                    return;
                case StateCode.PreFlop:
                    this.Fire(TexasTrigger.state_flop);
                    return;
                case StateCode.Flop:
                    this.Fire(TexasTrigger.state_turn);
                    return;
                case StateCode.Turn:
                    this.Fire(TexasTrigger.state_river);
                    return;
                case StateCode.River:
                    this.Fire(TexasTrigger.state_showdown);
                    return;
                case StateCode.ShowDown:
                    this.Fire(TexasTrigger.state_flip_card);
                    return;
                case StateCode.FlipCard:
                    this.Fire(TexasTrigger.state_calculating);
                    return;
                case StateCode.Calculating:
                    this.Fire(TexasTrigger.state_done);
                    return;
                case StateCode.Done:
                    this.Fire(TexasTrigger.state_waiting_new_game);
                    return;
            }
        }

        public override void OnRemove()
        {
            Queue.Stop();
            base.OnRemove();
        }

        public void Fire(TexasTrigger trigger)
        {
            // Log.DebugFormat("STATE MACHINE FIRE: " + trigger);
            States.Fire(trigger);
            Log.DebugFormat("CURRENT STATE: " + this.States.State);
        }

        /*
        public void Fire(TexasTrigger trigger)
        {
            SendNotification(trigger.ToString("G"), null);
        }
        */

        public void Fire(long delayTime)
        {
            // Queue.SetTimer(stateChanged, null, delayTime);
            // stateChanged();
        }

        
    }
}
