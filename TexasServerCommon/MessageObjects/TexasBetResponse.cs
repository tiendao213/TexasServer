using System;

namespace TexasServerCommon.MessageObjects
{
    [Serializable]
    public class TexasBetResponse
    {
        public long Credit;
        public long TotalMoneyOnTable;
        public long CurrentBet;
    }
}
