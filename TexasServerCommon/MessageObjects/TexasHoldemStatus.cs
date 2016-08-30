using System;
namespace TexasServerCommon.MessageObjects
{
    [Serializable]
    public class TexasHoldemStatus
    {
        public int TableState;
        public PlayerItem[] Players;
        public long[] ChipPot;
        public int[] CenterCards;
        public string CurrentPlaying;
        public int TimeRemain;
        public int DealerIndex;
        public int SmallBlindIndex;
        public int BigBlindIndex;
    }
}
