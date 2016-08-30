using System;


namespace TexasServerCommon.MessageObjects
{
    [Serializable]
    public class TexasGameInfo
    {
        public long[] ChipPots;
        public int[] CenterCards;
        public string CurrentPlaying;
        public int TableState;
    }
}
