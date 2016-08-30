using System;

namespace TexasServerCommon.MessageObjects
{
    [Serializable]
    public class PlayerItem
    {
        public string UserKey;
        public string UserName;
        public string AvatarKey;
        public long Credit;
        public long CurrentBet;
        // public long Bet;
        public long RemainTime;
        /**
         * 0: Waiting
         * 1: Fold
         * 2: Bet
         * 3: Raise
         * 4: Small Blind
         * 5: Big Blind
         * 6: Check
         * 7: All in
         * 8: Royal Flush
         * 9: Straight Flush
         * 10: Four of a kind
         * 11: Full House
         * 12: Flush
         * 13: Straight
         * 14: Three of a kind
         * 15: Two Pairs
         * 16: One Pair
         * 17: High Card
         * 18: Call
         * */
        public int Status;
        public long TotalMoneyOnTable;
        public bool isOnGame;
    }
}
