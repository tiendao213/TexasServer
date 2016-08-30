

namespace VuiLen.Texas.Game.StateMachine
{
    public enum TexasTrigger
    {
        state_waiting_new_game,
        state_start_game,
        state_play,
        state_pre_flop,
        state_flop,
        state_turn,
        state_river,
        state_showdown,
        state_flip_card,
        state_calculating,
        state_done,
        action_bet,
        action_sit,
        action_update_slot,

    }
}
