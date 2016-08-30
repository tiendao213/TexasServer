using System;

namespace TexasServerCommon.MessageObjects
{
    [Serializable]
    public class TexasCalculatingInfo
    {
        public WinInfo[] WinInfos;
        public PlayerWin PlayerWin;
        public CardWin CardWin;
    }
}
