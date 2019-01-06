using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class GameStateExport
    {
        #region Properties
        public int PlayerTurn { get; }
        public bool GameEnded { get; } 
        public int[,] Gameboard { get; }
        public (int, int) Scores { get; }
        public List<(int, int)> PossiblesMoves { get; }
        public (long, long) RemainingTimes { get; }

        #endregion

        public GameStateExport(bool gameEnded, int playerTurn, (int, int) scores, int[,] gameboard, List<(int, int)> possiblesMoves, (long, long) remainingTimes)
        {
            GameEnded = gameEnded;
            PlayerTurn = playerTurn;
            Scores = scores;
            Gameboard = gameboard;
            PossiblesMoves = possiblesMoves;
            RemainingTimes = remainingTimes;
        }
    }
}
