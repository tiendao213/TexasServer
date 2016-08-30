using System;
using System.Collections.Generic;
using System.Timers;
using Texas.Game.Models.vos;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;
using VuiLen.Texas.Game.Models;
using VuiLen.TexasServer.game.Controllers.Room;
using VuiLen.TexasServer.game.Models.vos;
using VuiLen.TexasServer.Handlers;
using VuiLen.Utilities;
using VuiLenGameFramework.MVC.Patterns;
using VuiLenGameFramework.Room.Models;
using VuiLenServerCommon.MessageObjects;

namespace VuiLen.Texas.Game.Models
{
    public class TexasModel : Proxy
    {
        #region Members
        new public const string NAME = "TexasModel";
        #endregion
        public List<Card> Cards;

        public int TableState { get; private set; }
        public int BigBlindIndex { get; private set; }
        public int SmallBlindIndex { get; private set; }
        public int DealerIndex { get; private set; }

        

        public Card[] CardsOnTable;
        public List<ChipPot> ChipPots;
        public Slot PrevPlayer;
        public Slot CurrentPlayer;
        public Slot[] Slots;
        public Slot Starter;
        // private long countDownTime;
        private const int MAX_PLAYER_SIZE = 9;
        private List<Card> RemainCard;
        private long CurrentBet;
        private ChipPot currentChipPot;
        private TimerQueue thinkingTimer;
        // private IntervalTimer intervalTimer;

        public long SmallBlindValue = 1000;
        private const int MIN_PLAYER_REQUIVEMENT = 4;

        internal bool isEnoughtPlayer()
        {
            return this.getPlayerCount() >= MIN_PLAYER_REQUIVEMENT;
        }

        internal Slot getPrevPlayer()
        {
            return this.PrevPlayer;
        }

        internal Slot getCurrentPlayer()
        {
            if (this.CurrentPlayer == null || this.CurrentPlayer.isEmpty())
            {
                Log.DebugFormat("-WARNING: CURRENT PLAYER IS NULL");
                return null;
            }
            return this.CurrentPlayer;
        }

        internal Slot[] getSlots()
        {
            return this.Slots;
        }   

        public long[] buildMinBets()
        {

            long[] minsBets = new long[this.Slots.Length];
            if (this.CurrentBet == 0)
            {
                return minsBets;
            }
            for (int index = 0; index < this.Slots.Length; index ++)
            {
                Slot slot = this.Slots[index];
                if (slot.isEmpty())
                {
                    minsBets[index] = 0;
                    continue;
                }
                if (!slot.isOnGame)
                {
                    minsBets[index] = 0;
                    continue;
                }
                if (slot.isFold)
                {
                    minsBets[index] = 0;
                    continue;
                }
                minsBets[index] = this.CurrentBet - slot.CurrentBet;
            }
            return minsBets;
        }

        internal void bet(Slot slot, TexasBetRequest texasBetRequest)
        {
            betInternal(slot, texasBetRequest);
            this.PrevPlayer = slot;
            Log.DebugFormat("DONE BET, PREV PLAYER = " + this.PrevPlayer.UserKey);
            int slotIndex = getSlotIndex(slot);
            Slot nextPlayer = getNextPlayer(slotIndex + 1);

            updateTimeForThink(nextPlayer);
            if (nextPlayer == this.Starter)
            {
                calcPot();
                // update state
                this.TableState++;
                // reset new bet
                this.CurrentBet = 0;
                // add to pot on table
                this.ChipPots.Add(this.currentChipPot);
                // create new pot
                this.currentChipPot = new ChipPot();

                // clear new prevPlayer
                this.PrevPlayer = null;
                Log.DebugFormat("Next State: " + this.TableState);
            }
            this.CurrentPlayer = nextPlayer;
            Log.DebugFormat("DONE BET, CURRENT PLAYER = " + this.CurrentPlayer.UserKey);
            if (this.TableState > 6)
            {
                // handle and game
                return;
            }

        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var texasModel = source as TexasModel;
            
        }

        private void calcPot()
        {
        }

        private void betInternal(Slot slot, TexasBetRequest texasBetRequest)
        {
            Log.DebugFormat("BET INFO: UserKey:" + texasBetRequest.UserKey + " BetType:" + texasBetRequest.BetType + " Amount:" + texasBetRequest.amount 
                + " CurrentCredit:" + slot.CurrentCredit + " CurrentBet:" + slot.CurrentBet + " getCurrentBet:" + this.getCurrentBet() + " SmallBlind:" 
                + this.SmallBlindValue);

            // if fold
            if (texasBetRequest.BetType == (int)BetTypeCode.Fold)
            {
                fold(slot);
                return;
            }
            // if not enought money to bet. force all in
            if (slot.CurrentCredit + slot.CurrentBet < getCurrentBet() + SmallBlindValue)
            {
                // force all in
                allIn(slot);
                return;
            }
            // all in
            if (texasBetRequest.BetType == (int)BetTypeCode.AllIn)
            {
                allIn(slot);
                return;
            }
            // not amount
            if (texasBetRequest.amount == 0) // check
            {
                if (getCurrentBet() > 0)
                {
                    call(slot);
                    return;
                }
                check(slot);
                return;
            }
            // out of current credit
            if (texasBetRequest.amount >= slot.CurrentCredit)
            {
                allIn(slot);
                return;
            }
            // 0 < amount < slot.getCurrentCredit
            if (getCurrentBet() == 0)
            {
                bet(slot, texasBetRequest.amount);
                return;
            }
            if (getCurrentBet() >= slot.CurrentBet + texasBetRequest.amount)
            {
                call(slot);
                return;
            }
            raise(slot, texasBetRequest.amount);
        }

        private void raise(Slot slot, long amount)
        {
            // setstatus
            slot.Status = (int)PlayerStatusCode.Raise;
            // set starter
            this.Starter = slot;
            // update current on table
            this.CurrentBet = slot.CurrentCredit + amount;
            // update chippot
            this.currentChipPot.updatePot(amount, slot.UserKey);
            // get credit from slot
            slot.CurrentCredit -= amount;
            // put to current bet
            slot.CurrentBet += amount;
        }

        private void bet(Slot slot, long amount)
        {
            slot.Status = (int)PlayerStatusCode.Bet;
            if (this.CurrentBet < slot.CurrentCredit + amount)
            {
                this.CurrentBet = slot.CurrentCredit + amount;
                this.Starter = slot;
            }
            this.currentChipPot.updatePot(amount, slot.UserKey);
            slot.CurrentCredit -= amount;
            slot.CurrentBet += amount;
        }

        private void call(Slot slot)
        {
            var amount = getCurrentBet() - slot.CurrentBet;
            if (amount > 0)
            {
                slot.Status = (int)PlayerStatusCode.Call;
                this.currentChipPot.updatePot(amount, slot.UserKey);
                slot.CurrentCredit -= amount;
                slot.CurrentBet += amount;
                return;
            }
            check(slot);
        }

        private void check(Slot slot)
        {
            slot.Status = (int)PlayerStatusCode.Check;
        }

        private void fold(Slot slot)
        {
            slot.Status = (int) PlayerStatusCode.Fold;
            slot.isFold = true;
            int currentPlayerOnGameCount = getCurrentPlayerOnGameCount();
            if (currentPlayerOnGameCount == 1)
            {
                endGame(false);
            }
        }

        private void endGame(bool isFlip)
        {
            this.TableState = (int) GameStateCode.ShowDown;
            // TODO Fix:
        }

        private int getCurrentPlayerOnGameCount()
        {
            int count = 0;
            foreach (Slot slot in this.Slots)
            {
                if (slot.isOnGame && !slot.isFold)
                {
                    count++;
                }
            }
            return count;
        }

        private void allIn(Slot slot)
        {
            slot.Status = (int)PlayerStatusCode.AllIn;
            if (this.CurrentBet < slot.CurrentCredit)
            {
                this.CurrentBet = slot.CurrentCredit;
                this.Starter = slot;
            }
            this.currentChipPot.updatePot(slot.CurrentCredit, slot.UserKey);
            slot.CurrentCredit = 0;
            slot.CurrentBet += slot.CurrentCredit;
        }

             internal long getCurrentBet()
        {
            return this.CurrentBet;
        }
            

        internal TexasGameInfo buildTexasGameInfo()
        {
            TexasGameInfo texasGameInfo = new TexasGameInfo();
            texasGameInfo.ChipPots = buildChipPotValues();
            texasGameInfo.CenterCards = buildCenterCardIds();
            texasGameInfo.CurrentPlaying = this.CurrentPlayer.UserKey;
            texasGameInfo.TableState = this.TableState;
            return texasGameInfo;
        }

        internal void startGame()
        {
            Log.DebugFormat("SETUP START GAME");
            int currentIndex = this.DealerIndex;
            Slot smallBlindSlot = getNextPlayer(currentIndex + 1);
            if (smallBlindSlot == null) // no one on table
            {
                return;
            }

            currentIndex = getSlotIndex(smallBlindSlot);
            if (currentIndex < 0)
            {
                return;
            }
            Slot bigBlindSlot = getNextPlayer(currentIndex + 1);
            if (bigBlindSlot == null) // no one on table
            {
                return;
            }

            var texasModel = this.Facade.RetrieveProxy(TexasModel.NAME) as TexasModel;

            // small blind
            smallBlindSlot.CurrentBet = this.SmallBlindValue;
            smallBlindSlot.CurrentCredit -= this.SmallBlindValue;
            smallBlindSlot.Status = (int) PlayerStatusCode.SmallBlind;
            this.SmallBlindIndex = getSlotIndex(smallBlindSlot);

            // big blind
            bigBlindSlot.CurrentBet = this.SmallBlindValue * 2;
            bigBlindSlot.CurrentCredit -= this.SmallBlindValue * 2;
            bigBlindSlot.Status = (int)PlayerStatusCode.BigBlind;
            this.BigBlindIndex = getSlotIndex(bigBlindSlot);

            currentIndex = this.BigBlindIndex;

            // find nextPlayer
            Slot currentPlayer = getNextPlayer(currentIndex + 1);
            this.CurrentPlayer = currentPlayer;
            this.PrevPlayer = null;
            this.Starter = currentPlayer;

            this.CurrentBet = this.SmallBlindValue * 2;
            // big blind + small blind
            currentChipPot = new ChipPot();
            currentChipPot.updatePot(SmallBlindValue, smallBlindSlot.UserKey);
            currentChipPot.updatePot(SmallBlindValue * 2, bigBlindSlot.UserKey);

            this.TableState = (int) GameStateCode.PreFlop;

            // set time for think
            updateTimeForThink(currentPlayer);
            
            for (int index = 0; index< this.Slots.Length; index ++)
            {
                Slot slot = this.Slots[index];
                if (slot.isEmpty())
                {
                    continue;
                }
                slot.isOnGame = true;
                slot.isFold = false;
            }

            this.isPlaying = true;
            Log.DebugFormat("SETUP START GAME DONE");
        }

        private void updateTimeForThink(Slot currentPlayer)
        {
            foreach (Slot slot in this.Slots)
            {
                slot.clearTime();
            }
            currentPlayer.updateTimeForThink();
            Log.DebugFormat("TIMER STEP 1");
            
            this.thinkingTimer.SetTimer(timeForThinkCallBack, null, 3000);
            Log.DebugFormat("TIMER STEP 3");
        }

        private void timeForThinkCallBack(object s)
        {
            Log.DebugFormat("TIMER STEP CALLBACK");
            if (this.CurrentPlayer == null)
            {
                return;
            }
            TexasBetRequest texasBetRequest = new TexasBetRequest();
            texasBetRequest.amount = 0;
            texasBetRequest.BetType = (int) BetTypeCode.Fold;
            texasBetRequest.UserKey = this.CurrentPlayer.UserKey;
            bet(this.CurrentPlayer, texasBetRequest);
            Log.DebugFormat("TIMER STEP END CALLBACK");
        }

        public int getSlotIndex(Slot slot)
        {
            for (int index = 0; index < this.Slots.Length; index ++)
            {
                if (this.Slots[index] == slot)
                {
                    return index;
                }
            }
            return -1;
        }

        private Slot getNextPlayer(int currentIndex)
        {
            for (int index = 0; index < this.Slots.Length; index++)
            {
                Slot slot = getSlot(currentIndex + index);
                if (!slot.isEmpty())
                {
                    return slot;
                }
            }
            return null;
        }

        private int getPlayerCount()
        {
            int result = 0;
            foreach (var slot in this.Slots){
                if (!slot.isEmpty())
                {
                    result++;
                }
            }
            return result;
        }

        private Dictionary<string, TexasRoomUser> TexasRoomUsers;
        public bool isPlaying { get; internal set; }

        public TexasModel(): base(NAME)
        {
            initCard();
            initDefaultValue();
            TexasRoomUsers = new Dictionary<string, TexasRoomUser>();
            initSlot();
            thinkingTimer = new TimerQueue(1);
            thinkingTimer.Initialize();
        }

        private void initSlot()
        {
            this.Slots = new Slot[MAX_PLAYER_SIZE];
            for (int index = 0; index < MAX_PLAYER_SIZE; index ++)
            {
                Slot slot = new Slot();
                slot.emptySlot();
                this.Slots[index] = slot;
            }
        }

        public void joinRoom(UserItem userItem)
        {
            if (TexasRoomUsers.ContainsKey(userItem.UserKey))
            {
                TexasRoomUser texasRoomUser = TexasRoomUsers[userItem.UserKey];
                texasRoomUser.UserName = userItem.UserName;
                Log.DebugFormat("WELCOME BACK(" + texasRoomUser.UserKey + ") TO VL");
            } else
            {
                TexasRoomUser texasRoomUser = new TexasRoomUser();
                texasRoomUser.UserKey = userItem.UserKey;
                texasRoomUser.UserName = userItem.UserName;
                TexasRoomUsers[texasRoomUser.UserKey] = texasRoomUser;
                Log.DebugFormat("WELCOME " + texasRoomUser.UserKey + " TO VL");
            }
        }

        public bool sit(TexasRoomUser texasRoomUser, int index, TexasSitDownListener listener)
        {
            if (texasRoomUser == null)
            {
                return false;
            }
            if (!this.Slots[index].isEmpty())
            {
                return false; // this slot ready busy
            }
            Slot slot = this.Slots[index];
            slot.UserKey = texasRoomUser.UserKey;
            slot.UserName = texasRoomUser.UserName;
            sendSitDownInfoToAllUserInRoom(slot, index, true, listener);
            return true;
        }

        internal bool sit(string userKey, int index, TexasSitDownListener listener)
        {
            TexasRoomUser texasRoomUser = getTexasRoomUser(userKey);
            if (texasRoomUser == null)
            {
                return false;
            }
            return sit(texasRoomUser, index, listener);
        }

        private TexasRoomUser getTexasRoomUser(string userKey)
        {
            if (this.TexasRoomUsers.ContainsKey(userKey))
            {
                return this.TexasRoomUsers[userKey];
            }
            return null;
        }

        public void unsit(string UserKey, TexasSitDownListener listener)
        {
            for (int index = 0; index < this.Slots.Length; index ++)
            {
                if (this.Slots[index].isEmpty())
                {
                    continue;
                }
                if (this.Slots[index].UserKey == UserKey)
                {
                    unsit(this.Slots[index], index, listener);
                    return;
                }
            }
        }

        public void unsit(Slot player, TexasSitDownListener listener)
        {
            for (int index = 0; index < this.Slots.Length; index ++)
            {
                if (player != this.Slots[index])
                {
                    continue;
                }
                unsit(player, index, listener);
                return;
            }
        }

        public void unsit(Slot player, int index, TexasSitDownListener listener)
        {
            setLose(player);
            // push sit downinfo to all user;
            sendSitDownInfoToAllUserInRoom(player, index, false, listener);
            // Slots[index].emptySlot();
            player.emptySlot();
        }

        internal void forceUnsitIfNotEnoughtMoney(TexasSitDownListener listener)
        {
            for (int index = 0; index < this.Slots.Length; index ++)
            {
                Slot slot = this.Slots[index];
                if (slot.isEmpty())
                {
                    continue;
                }
                if (slot.CurrentCredit < this.SmallBlindValue * 2) // Big Blind
                {
                    unsit(slot, index, listener);
                }
            }
        }

        private void sendSitDownInfoToAllUserInRoom(Slot player, int index, bool isSit, TexasSitDownListener listener)
        {
            if (listener ==null)
            {
                return;
            }
            SitDownInfo sitDownInfo = new SitDownInfo();
            sitDownInfo.AvatarKey = player.AvatarKey;
            sitDownInfo.Index = index;
            sitDownInfo.IsSitDown = isSit;
            sitDownInfo.UserKey = player.UserKey;
            sitDownInfo.UserName = player.UserName;
            TexasSitDown texasSitDown = new TexasSitDown();
            texasSitDown.sisDownInfo = sitDownInfo;
            listener.PushSitDownNotification(texasSitDown);
        }

        public Slot getSlot(int index)
        {
            return this.Slots[index % this.Slots.Length];
        }

        public Slot getSlot(String userKey)
        {
            for (int index = 0; index < this.Slots.Length; index ++)
            {
                Slot player = this.Slots[index];
                if (player.isEmpty())
                {
                    continue;
                }
                if (player.UserKey == userKey)
                {
                    return player;
                }
            }
            return null;
        }

        public Slot leaveRoom(string UserKey, TexasSitDownListener listener)
        {
            if (this.TexasRoomUsers.ContainsKey(UserKey))
            {
                TexasRoomUsers.Remove(UserKey) ;
            }

            // check if playing
            for (int index = 0; index < this.Slots.Length; index ++) {
                Slot player = this.Slots[index];
                if (player.isEmpty()) // this is empty slot
                {
                    continue;
                }
                if (player.UserKey == UserKey)
                {
                    unsit(player, index, listener);
                    return player;
                    // break;
                }
            }
            return null;
        }

        private void setLose(Slot player)
        {
            if (!player.isOnGame)
            {
                return;
            }
            fold(player);
        }

        private void initDefaultValue()
        {
            this.CardsOnTable = new Card[5];
            ChipPots = new List<ChipPot>();
        }

        private void initCard()
        {
            Cards = new List<Card>();
            int count = 10;
            createCard(Cards, count);
            count = 20;
            createCard(Cards, count);
            count = 30;
            createCard(Cards, count);
            count = 40;
            createCard(Cards, count);
            count = 50;
            createCard(Cards, count);
            count = 60;
            createCard(Cards, count);
            count = 70;
            createCard(Cards, count);
            count = 80;
            createCard(Cards, count);
            count = 90;
            createCard(Cards, count);
            count = 100;
            createCard(Cards, count);
            count = 110;
            createCard(Cards, count);
            count = 120;
            createCard(Cards, count);
            count = 130;
            createCard(Cards, count);
        }

        private void ShuftCard()
        {
            RemainCard = new List<Card>(this.Cards);
            Log.DebugFormat("SHUFT CARD: " + RemainCard.Count);
            Random rnd = new Random();
            List<Card> cards = new List<Card>();
            for (int index = 0; index < this.Cards.Count; index ++) {
                int random = rnd.Next(0, RemainCard.Count);
                cards.Add(RemainCard[random]);
                RemainCard.RemoveAt(random);
            }
            this.RemainCard = cards;
            
            // for slot
            for (int index = 0; index < this.Slots.Length; index ++)
            {
                Slot slot = this.Slots[index];
                slot.Cards[0] = this.RemainCard[0];
                slot.Cards[1] = this.RemainCard[1];
                this.RemainCard.RemoveAt(0);
                this.RemainCard.RemoveAt(0);
            }
            // for card on table
            for (int index = 0; index < 5; index ++)
            {
                this.CardsOnTable[index] = this.RemainCard[0];
                this.RemainCard.RemoveAt(0);
            }
            
            Log.DebugFormat("RemainCard CARD: " + this.RemainCard.Count);
        }

        internal void reset()
        {
            Log.DebugFormat("RESET FOR NEW GAME");
            this.isPlaying = false;
            for (int index = 0; index < this.Slots.Length; index++)
            {
                Slot slot = this.Slots[index];
                slot.Status = (int) PlayerStatusCode.Waiting;
            }
            ShuftCard();
        }

        public TexasHoldemStatus buildTexasHoldemStatus()
        {
            TexasHoldemStatus texasHoldemStatus = new TexasHoldemStatus();
            texasHoldemStatus.BigBlindIndex = this.BigBlindIndex;
            texasHoldemStatus.CenterCards = buildCenterCardIds();
            texasHoldemStatus.ChipPot = buildChipPotValues();
            if (this.CurrentPlayer != null)
            {
                texasHoldemStatus.CurrentPlaying = CurrentPlayer.UserKey;
            }
            texasHoldemStatus.DealerIndex = this.DealerIndex;
            texasHoldemStatus.Players = buildPlayers();
            texasHoldemStatus.SmallBlindIndex = this.SmallBlindIndex;
            texasHoldemStatus.TableState = this.TableState;

            /*
            var time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            texasHoldemStatus.TimeRemain = (int)(time - this.countDownTime) / 1000;
            texasHoldemStatus.TimeRemain = BET_TIME - texasHoldemStatus.TimeRemain;
            */

            return texasHoldemStatus;
        }

        private PlayerItem[] buildPlayers()
        {
            PlayerItem[] PlayerItems = new PlayerItem[this.Slots.Length];
            for (int index = 0; index < this.Slots.Length; index ++)
            {
                var slot = this.Slots[index];
                if (slot.isEmpty())
                {
                    continue;
                }
                PlayerItem playerItem = slot.build();
                PlayerItems[index] = playerItem;
            }
            return PlayerItems;
        }

        private long[] buildChipPotValues()
        {
            long[] chipPotValues = new long[this.ChipPots.Count];
            for (int index = 0; index < this.ChipPots.Count; index ++)
            {
                chipPotValues[index] = this.ChipPots[index].Amount;
            }
            return chipPotValues;
        }

        private int[] buildCenterCardIds()
        {
            int[] centerCardIds = new int[this.CardsOnTable.Length];
            for (int index = 0; index < this.CardsOnTable.Length; index ++)
            {
                if (this.CardsOnTable[index] == null)
                {
                    centerCardIds[index] = 0;
                } else
                {
                    centerCardIds[index] = this.CardsOnTable[index].id;
                }
            }
            return centerCardIds;
        }

        private void createCard(List<Card> Cards, int count)
        {
            Cards.Add(new Card(count + 0, (int)CardCode.Spades, count / 10));
            Cards.Add(new Card(count + 1, (int)CardCode.Clubs, count / 10));
            Cards.Add(new Card(count + 2, (int)CardCode.Diamonds, count / 10));
            Cards.Add(new Card(count + 3, (int)CardCode.Hearts, count / 10));
        }
    }
}