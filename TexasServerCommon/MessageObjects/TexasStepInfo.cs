using System;


namespace TexasServerCommon.MessageObjects
{
    [Serializable]
    public class TexasStepInfo
    {
        public PlayerItem CurrentPlayer;
        public PlayerItem PrevPlayer;
    }
}
