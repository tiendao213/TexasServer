using System;

namespace TexasServerCommon.MessageObjects
{
    [Serializable]
    public class TexasBetRequest
    {
        public string UserKey;
        /**
         * 0: Fold
        1: Check
        2: Call
        3: Raise
        4: AllIn */
        public int BetType;
        public long amount;
    }
}
