using System;

namespace TexasServerCommon.MessageObjects
{
    [Serializable]
    public class SitDownInfo
    {
        public string UserKey;
        public string UserName;
        public string AvatarKey;
        public int Index;
        public bool IsSitDown;
    }
}
