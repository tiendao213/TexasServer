using System;


namespace TexasServerCommon.MessageObjects
{
    [Serializable]
    public class BuyinInfo
    {
        public string UserKey;
        public string UserName;
        public string AvatarKey;
        public int Index;
        public long Credit;
    }
}
