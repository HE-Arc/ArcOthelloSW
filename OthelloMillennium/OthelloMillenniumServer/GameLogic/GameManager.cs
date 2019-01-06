using NUnit.Framework;
using OthelloMillenniumServer.GameLogic;
using System;
using System.Collections.Generic;
using Tools;

namespace OthelloMillenniumServer
{
    public class GameManager
    {
        #region Internal Classes
        public class GameManagerArgs
        {
            public string Winner { get; private set; }

            public (long, long) TotalTime { get; private set; }

            public (int, int) Score { get; private set; }
        }

        public enum GameType
        {
            SinglePlayer = 0,
            MultiPlayer = 1
        }

        public enum Player
        { // Values have been choosen to fit GameState.CellState
            BlackPlayer = 1,
            WhitePlayer = 2
        }

        private static GameState.CellState PlayerToCellState(Player player)
        {
            return player == Player.BlackPlayer ? GameState.CellState.BLACK : GameState.CellState.WHITE;
        }

        #endregion

        #region Properties
        public bool GameEnded { get; private set; }
        public GameType Type { get; private set; }

        #endregion

        #region Attributes
        private Player CurrentPlayerTurn;
        private int indexState;
        private List<GameState> listGameState;
        private Dictionary<Player, StoppableTimer> timeCounter;
        private (int, int) scores;

        #endregion

        #region Events
        public event EventHandler<GameManagerArgs> OnGameFinished;

        #endregion

        public GameManager(GameType gameType)
        {
            Type = gameType;

            //Init GameState
            indexState = 0;
            listGameState = new List<GameState>();
            listGameState.Add(GameState.CreateStartState());
            Assert.False(listGameState[indexState].GameEnded);

            if (Type == GameType.MultiPlayer)
            {
                timeCounter = new Dictionary<Player, StoppableTimer>()
                {
                    { Player.BlackPlayer, new StoppableTimer(Settings.TimePerPlayer) },
                    { Player.WhitePlayer, new StoppableTimer(Settings.TimePerPlayer) }
                };
            }
            
            CurrentPlayerTurn = Player.BlackPlayer;

            ComputeScore();
        }

        /// <summary>
        /// Start the game counter
        /// </summary>
        public void Start()
        {
            if (GameEnded)
            {
                throw new Exception("Game ended");
            }

            //We start the counter
            if (Type == GameType.MultiPlayer)
            {
                timeCounter[Player.BlackPlayer].Start();
            }
        }

        /// <summary>
        /// Allow to play a move
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="isPlayerOne"></param>
        public void PlayMove((char, int) coord, Player player)
        {
            if (GameEnded)
            {
                throw new Exception("Game ended");
            }

            if (player != CurrentPlayerTurn)
            {
                throw new Exception("Invalid player turn");
            }

            if (Type == GameType.MultiPlayer)
            {
                timeCounter[CurrentPlayerTurn].Stop();
            }

            GameState.CellState cellStatePlayer = PlayerToCellState(CurrentPlayerTurn);
            listGameState[indexState].ValidateMove(coord, cellStatePlayer);

            //Manage the case when we have made a moveback
            if (indexState + 1 < listGameState.Count)
            {
                listGameState.RemoveRange(indexState+1, listGameState.Count-indexState-1);
            }

            listGameState.Add(listGameState[indexState].ApplyMove(coord, cellStatePlayer));
            ++indexState;

            SwitchPlayer();
            if (Type == GameType.MultiPlayer)
            {
                timeCounter[CurrentPlayerTurn].Start();
            }

            //Manage end of the game
            if (listGameState[indexState].GameEnded)
            {
                EndGame();
            }
            else
            {
                ComputeScore();
            }
        }
        
        /// <summary>
        /// Reverse a move
        /// </summary>
        public void MoveBack()
        {
            if (GameEnded)
            {
                throw new Exception("Game ended");
            }

            if (Type == GameType.MultiPlayer)
            {
                throw new Exception("Action not allowed in Multiplayer game type");
            }

            if (indexState > 0)
            {
                --indexState;
                SwitchPlayer();
            }
            else
            {
                throw new Exception("Not possible to move back");
            }
        }
        
        /// <summary>
        /// Cancel a move back
        /// </summary>
        public void MoveForward()
        {
            if (GameEnded)
            {
                throw new Exception("Game ended");
            }

            if (Type == GameType.MultiPlayer)
            {
                throw new Exception("Action not allowed in Multiplayer game type");
            }

            if (indexState < listGameState.Count - 1)
            {
                ++indexState;
                SwitchPlayer();
            }
            else
            {
                throw new Exception("Not possible to move forward");
            }
        }

        /// <summary>
        /// Event called when the game is over
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimeout(Object source, EventArgs e)
        {
            EndGame();
        }

        /// <summary>
        /// Switch the current player
        /// </summary>
        private void SwitchPlayer()
        {
            CurrentPlayerTurn = (Player)(((int)CurrentPlayerTurn) % 2) + 1;
        }

        /// <summary>
        /// End a game
        /// </summary>
        private void EndGame()
        {
            GameEnded = true;
            EventHandler<GameManagerArgs> handler = OnGameFinished;
            GameManagerArgs gameManagerArgs = new GameManagerArgs();

            timeCounter[CurrentPlayerTurn].Stop();
            ComputeScore();

            //TODO Complete event values
            handler(this, gameManagerArgs);
        }

        private void ComputeScore()
        {
            GameState gameState = listGameState[indexState];
            int maxScore = gameState.Gameboard.GetLength(0) * gameState.Gameboard.GetLength(1);
            
            if (timeCounter[Player.BlackPlayer].GetRemainingTime() == 0 || timeCounter[Player.WhitePlayer].GetRemainingTime() == 0)
            {
                //One player is out of time
                scores = timeCounter[Player.BlackPlayer].GetRemainingTime() == 0 ? (0, maxScore) : (maxScore, 0);
            }
            else
            {
                // Get nb token per player
                (int black, int white) = (gameState.getNbToken(GameState.CellState.BLACK), gameState.getNbToken(GameState.CellState.WHITE));

                if (!GameEnded || gameState.getNbToken(GameState.CellState.EMPTY) == 0)
                {
                    // Default Count number of token for each player
                    scores = (black, white);
                }
                else if (black == 0 || white == 0)
                {
                    //Eradication of a player
                    scores = black == 0 ? (0, maxScore) : (maxScore, 0);
                }
                else
                {
                    // No player can move
                    scores = black > white ? (maxScore - white, white) : (black, maxScore - black);
                }
            }
        }

        private GameStateExport Export()
        {
            GameState.CellState[,] gameboard = listGameState[indexState].Gameboard;
            int[,] board = new int[gameboard.GetLength(0), gameboard.GetLength(1)];

            for (int i=0; i< gameboard.GetLength(0); ++i)
            {
                for (int j = 0; j < gameboard.GetLength(1); ++j)
                {
                    board[i,j] = (int)gameboard[i,j];
                }
            }
            
            List<(int, int)> possiblesMoves = listGameState[indexState].PossibleMoves(PlayerToCellState(CurrentPlayerTurn));
            (long, long) remainingTimes = (timeCounter[Player.BlackPlayer].GetRemainingTime(), timeCounter[Player.WhitePlayer].GetRemainingTime());
            
            return new GameStateExport(GameEnded, (int)CurrentPlayerTurn, scores, board, possiblesMoves, remainingTimes);
        }
    }
}
