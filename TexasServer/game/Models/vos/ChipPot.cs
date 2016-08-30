

using System.Collections.Generic;

namespace VuiLen.TexasServer.game.Models.vos
{
    public class ChipPot
    {
        public long Amount { get; set; }
        public List<string> UserKeys { get; }

        public ChipPot(long Amount, List<string> UserKeys)
        {
            this.Amount = Amount;
            this.UserKeys = UserKeys;
        }

        public ChipPot()
        {
            this.UserKeys = new List<string>();
        }

        public ChipPot(long amount, string userKey)
        {
            this.UserKeys = new List<string>();
            this.UserKeys.Add(userKey);
            this.Amount = amount;
        }

        public void updatePot(long AmountPlus, string UserKey)
        {
            this.Amount += AmountPlus;
            if (!this.UserKeys.Contains(UserKey))
            {
                this.UserKeys.Add(UserKey);
            }
        }
    }
}
