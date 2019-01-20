using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Classes
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

    public enum Player
    { // Values have been choosen to fit GameState.CellState
        BlackPlayer = 1,
        WhitePlayer = 2
    }
}
