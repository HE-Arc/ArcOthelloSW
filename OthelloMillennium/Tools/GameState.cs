using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class GameState
    {
        #region Properties
        public int Winner { get; }
        public int PlayerTurn { get; }
        public bool GameEnded { get; } 
        public int[,] Gameboard { get; }
        public (int, int) Scores { get; }
        public List<(int, int)> PossiblesMoves { get; }
        public (long, long) RemainingTimes { get; }

        #endregion

        public GameState(bool gameEnded, int playerTurn, (int, int) scores, int[,] gameboard, List<(int, int)> possiblesMoves, (long, long) remainingTimes, int winner)
        {
            Scores = scores;
            Winner = winner;
            GameEnded = gameEnded;
            Gameboard = gameboard;
            PlayerTurn = playerTurn;
            PossiblesMoves = possiblesMoves;
            RemainingTimes = remainingTimes;
        }
    }
}
