
using System;
using Texas.Game.Models.vos;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;

namespace VuiLen.TexasServer.game.Models.vos
{
    public class Slot
    {
        private long TimeForThink;

        public int BET_TIME { get; }

        public bool isOnGame { get; set; }
        public bool isFold { get; set; }
        public string UserKey { get; internal set; }
        public string UserName { get; internal set; }
        public string AvatarKey { get; set; }
        public long CurrentCredit { get; set; }
        public long TotalCreditOnTable { get; set; }
        public long CurrentBet { get; set; }
        public int Status { get; set; }
        public Card[] Cards { get; set; }

        public Slot()
        {
            BET_TIME = 20;
            Cards = new Card[2];
        }

        internal PlayerItem build()
        {
            PlayerItem playerItem = new PlayerItem();
            playerItem.AvatarKey = AvatarKey;
            playerItem.Credit = CurrentCredit;
            playerItem.CurrentBet = CurrentBet;
            playerItem.RemainTime = getRemainTimeForThink();
            playerItem.Status = Status;
            playerItem.TotalMoneyOnTable = TotalCreditOnTable;
            playerItem.UserKey = UserKey;
            playerItem.UserName = UserName;
            playerItem.isOnGame = this.isOnGame;
            return playerItem;
        }

        public void emptySlot()
        {
            this.isOnGame = false;
            this.UserKey = null;
            this.UserName = null;
            this.AvatarKey = null;
            this.CurrentBet = 0;
            this.CurrentCredit = 0;
            this.TotalCreditOnTable = 0;
            this.Status = 0;
            this.isFold = false;
        }

        internal CardItem buildPrivateCard()
        {
            CardItem cardItem = new CardItem();
            cardItem.Card1 = this.Cards[0].id;
            cardItem.Card2 = this.Cards[1].id;
            return cardItem;
        }

        public void updateTimeForThink()
        {
            this.TimeForThink = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        private long getRemainTimeForThink()
        {
            var time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            time = (int)(time - this.TimeForThink) / 1000;
            time = BET_TIME - time;
            return time;
        }

        public void ResetForNextGame()
        {
            Status = (int) PlayerStatusCode.Waiting;
            CurrentBet = 0;
            TotalCreditOnTable = 0;
            this.isFold = false;
            this.Cards[0] = null;
            this.Cards[1] = null;
        }

        internal bool isEmpty()
        {
            return this.UserKey == null;
        }

        internal void clearTime()
        {
            this.TimeForThink = 0;
        }
    }
}
