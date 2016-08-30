using System;

namespace TexasServerCommon.Codes
{
    [Flags]
    public enum TexasOperationCode
    {
        Bet = 100,
        Status,
        Set,
        WaitingNewGame,
        StartGame,
        Sit,
    }
}
