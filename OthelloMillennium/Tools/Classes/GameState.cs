using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tools
{
    [Serializable]
    public class GameState : ISerializable
    {
        #region Properties
        public int Winner { get; }
        public int PlayerTurn { get; }
        public bool GameEnded { get; } 
        public int[,] Gameboard { get; }
        public Tuple<int, int> Scores { get; }
        public List<Tuple<int, int>> PossiblesMoves { get; }
        public Tuple<long, long> RemainingTimes { get; }

        #endregion

        public GameState(bool gameEnded, int playerTurn, Tuple<int, int> scores, int[,] gameboard, List<Tuple<int, int>> possiblesMoves, Tuple<long, long> remainingTimes, int winner)
        {
            Scores = scores;
            Winner = winner;
            GameEnded = gameEnded;
            Gameboard = gameboard;
            PlayerTurn = playerTurn;
            PossiblesMoves = possiblesMoves;
            RemainingTimes = remainingTimes;
        }

        protected GameState(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            Scores = (Tuple<int, int>)info.GetValue("Scores", typeof(Tuple<int, int>));
            Winner = (int)info.GetValue("Winner", typeof(int));
            GameEnded = (bool)info.GetValue("GameEnded", typeof(bool));
            Gameboard = (int[,])info.GetValue("Gameboard", typeof(int[,]));
            PlayerTurn = (int)info.GetValue("PlayerTurn", typeof(int));
            PossiblesMoves = (List<Tuple<int, int>>)info.GetValue("PossiblesMoves", typeof(List<Tuple<int, int>>));
            RemainingTimes = (Tuple<long, long>)info.GetValue("RemainingTimes", typeof(Tuple<long, long>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("Scores", Scores);
            info.AddValue("Winner", Winner);
            info.AddValue("GameEnded", GameEnded);
            info.AddValue("Gameboard", Gameboard);
            info.AddValue("PlayerTurn", PlayerTurn);
            info.AddValue("PossiblesMoves", PossiblesMoves);
            info.AddValue("RemainingTimes", RemainingTimes);
        }
    }
}
