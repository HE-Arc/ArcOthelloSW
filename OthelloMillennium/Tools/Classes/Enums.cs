using System;

namespace Tools
{
    public enum GameType
    {
        Local = 0,
        Online = 1,
    }

    public enum BattleType
    {
        AgainstAI = 0,
        AgainstPlayer = 1,
    }

    public enum PlayerType
    {
        Human = 2,
        AI = 3,
        Any = 1,

        // Used only on the server side
        None = 0,
    }

    public enum Color
    { // Values have been choosen to fit GameState.CellState
        Black = 1,
        White = 2
    }

    public enum PlayerState
    {
        INITIAL,
        REGISTERING,
        REGISTERED,
        SEARCHING,
        BINDED,
        LOBBY_CHOICE,
        READY,
        ABOUT_TO_START,
        OPPONENT_TURN,
        MY_TURN,
        GAME_ENDED
    }

}
