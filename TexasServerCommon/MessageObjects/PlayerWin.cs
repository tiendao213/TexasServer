using System;

namespace TexasServerCommon.MessageObjects
{
    [Serializable]
    public class PlayerWin
    {
        public string UserKey;
        public int Index;
        public CardWin CardWin;
        public long CreditWin;
        public long Credit;
    }
}
