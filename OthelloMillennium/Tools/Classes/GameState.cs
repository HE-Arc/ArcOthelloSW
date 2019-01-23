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
        public int TurnNumber { get; }
        public int NbTurn { get; }
        public Tuple<int, int> Scores { get; }
        public List<Tuple<char, int>> PossiblesMoves { get; }
        public Tuple<long, long> RemainingTimes { get; }

        #endregion

        public GameState(bool gameEnded, int playerTurn, Tuple<int, int> scores, int[,] gameboard, List<Tuple<char, int>> possiblesMoves, Tuple<long, long> remainingTimes, int winner, int nbTurn, int turnNumber)
        {
            Scores = scores;
            Winner = winner;
            NbTurn = nbTurn;
            GameEnded = gameEnded;
            Gameboard = gameboard;
            PlayerTurn = playerTurn;
            TurnNumber = turnNumber;
            PossiblesMoves = possiblesMoves;
            RemainingTimes = remainingTimes;
        }

        protected GameState(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            Scores = (Tuple<int, int>)info.GetValue("Scores", typeof(Tuple<int, int>));
            Winner = info.GetInt32("Winner");
            NbTurn = info.GetInt32("NbTurn");
            TurnNumber = info.GetInt32("TurnNumber");
            GameEnded = (bool)info.GetValue("GameEnded", typeof(bool));
            Gameboard = (int[,])info.GetValue("Gameboard", typeof(int[,]));
            PlayerTurn = info.GetInt32("PlayerTurn");
            PossiblesMoves = (List<Tuple<char, int>>)info.GetValue("PossiblesMoves", typeof(List<Tuple<char, int>>));
            RemainingTimes = (Tuple<long, long>)info.GetValue("RemainingTimes", typeof(Tuple<long, long>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("Scores", Scores);
            info.AddValue("Winner", Winner);
            info.AddValue("NbTurn", NbTurn);
            info.AddValue("GameEnded", GameEnded);
            info.AddValue("Gameboard", Gameboard);
            info.AddValue("PlayerTurn", PlayerTurn);
            info.AddValue("TurnNumber", TurnNumber);
            info.AddValue("PossiblesMoves", PossiblesMoves);
            info.AddValue("RemainingTimes", RemainingTimes);
        }
    }
}
