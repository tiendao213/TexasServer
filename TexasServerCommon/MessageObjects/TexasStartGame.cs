using System;

namespace TexasServerCommon.MessageObjects
{
    [Serializable]
    public class TexasStartGame
    {
        public int TableState;
        public int DealderIndex;
        public int SmallBlindIndex;
        public int BigBlindIndex;
    }
}
