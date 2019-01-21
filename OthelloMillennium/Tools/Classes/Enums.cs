using System;

namespace Tools
{
    public enum GameType
    {
        Local,
        Online,
    }

    public enum BattleType
    {
        AgainstAI,
        AgainstPlayer,
    }

    public enum PlayerType
    {
        Human,
        AI,
        Any,

        // Used only on the server side
        None,
    }

    public enum Color
    { // Values have been choosen to fit GameState.CellState
        Black = 1,
        White = 2
    }
}
