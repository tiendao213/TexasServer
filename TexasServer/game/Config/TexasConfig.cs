using VuiLenServerCommon.MessageObjects;

namespace VuiLen.TexasServer.Game.Config
{
    public class TexasConfig
    {

        public RoomItem Room { get; set; }

        public TexasConfig(
            string name,
            string description,
            int level,
            bool isPersistence,
            bool isPrivate,
            string key,
            string owner,
            long withdrawMin,
            long withdrawMax
            )
        {
            Room = new RoomItem()
            {

                Name = name,
                Description = description,
                Level = level,

                IsPersistence = isPersistence,
                IsPrivate = isPrivate,
                Key = key,
                Owner = owner,
                WithdrawMin = withdrawMin,
                WithdrawMax = withdrawMax


            };

        }
    }
}
