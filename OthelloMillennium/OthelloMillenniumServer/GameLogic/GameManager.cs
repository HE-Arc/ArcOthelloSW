using NUnit.Framework;
using OthelloMillenniumServer.GameLogic;
using System;
using System.Collections.Generic;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumServer
{
    public class GameManager
    {
        #region Internal Classes
        
        private static GameBoard.CellState PlayerToCellState(Player player)
        {
            return player == Player.BlackPlayer ? GameBoard.CellState.BLACK : GameBoard.CellState.WHITE;
        }

        #endregion

        #region Properties
        public bool GameEnded { get; private set; }
        public BattleType BattleType { get; private set; }
        public Player CurrentPlayerTurn { get; private set; }

        #endregion

        #region Attributes
        private int indexState;
        private List<GameBoard> listGameBoard;
        private readonly Dictionary<Player, StoppableTimer> timeCounter = new Dictionary<Player, StoppableTimer>();
        private Tuple<int, int> scores;
        private Player winner;

        #endregion

        #region Events
        public event EventHandler<GameState> OnGameFinished;

        #endregion

        public GameManager(BattleType battleType)
        {
            BattleType = battleType;

            //Init GameState
            indexState = 0;
            listGameBoard = new List<GameBoard>
            {
                GameBoard.CreateStartState()
            };

            Assert.False(listGameBoard[indexState].GameEnded);
            Assert.True(listGameBoard[indexState].LastPlayer == GameBoard.CellState.WHITE);

            if (BattleType == BattleType.AgainstPlayer)
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
            if (BattleType == BattleType.AgainstPlayer)
            {
                timeCounter[Player.BlackPlayer].Start();
            }
        }

        /// <summary>
        /// Allow to play a move
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="isPlayerOne"></param>
        public void PlayMove(Tuple<char, int> coord, Player player)
        {
            if (GameEnded)
            {
                throw new Exception("Game ended");
            }

            if (player != CurrentPlayerTurn)
            {
                throw new Exception("Invalid player turn");
            }

            if (BattleType == BattleType.AgainstPlayer)
            {
                timeCounter[CurrentPlayerTurn].Stop();
            }

            GameBoard.CellState cellStatePlayer = PlayerToCellState(CurrentPlayerTurn);
            listGameBoard[indexState].ValidateMove(coord, cellStatePlayer);

            //Manage the case when we have made a moveback
            if (indexState + 1 < listGameBoard.Count)
            {
                listGameBoard.RemoveRange(indexState+1, listGameBoard.Count-indexState-1);
            }

            listGameBoard.Add(listGameBoard[indexState].ApplyMove(coord, cellStatePlayer));
            ++indexState;

            SwitchPlayer();
            if (BattleType == BattleType.AgainstPlayer)
            {
                timeCounter[CurrentPlayerTurn].Start();
            }

            //Manage end of the game
            if (listGameBoard[indexState].GameEnded)
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

            if (BattleType == BattleType.AgainstPlayer)
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

            if (BattleType == BattleType.AgainstPlayer)
            {
                throw new Exception("Action not allowed in Multiplayer game type");
            }

            if (indexState < listGameBoard.Count - 1)
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
            GameBoard.CellState lastPlayer = listGameBoard[indexState].LastPlayer;
            GameBoard.CellState nextPlayer = (GameBoard.CellState)((int)lastPlayer % 2 + 1);
            CurrentPlayerTurn = (Player) (listGameBoard[indexState].PlayerCanPlay(nextPlayer) ? nextPlayer : lastPlayer);
        }

        /// <summary>
        /// End a game
        /// </summary>
        private void EndGame()
        {
            GameEnded = true;
            
            timeCounter[CurrentPlayerTurn].Stop();
            ComputeScore();
            winner = scores.Item1 > scores.Item2 ? Player.BlackPlayer : Player.WhitePlayer;

            EventHandler<GameState> handler = OnGameFinished;
            handler(this, Export());
        }

        private void ComputeScore()
        {
            GameBoard gameState = listGameBoard[indexState];
            int maxScore = gameState.Board.GetLength(0) * gameState.Board.GetLength(1);
            
            if (BattleType == BattleType.AgainstPlayer && (timeCounter[Player.BlackPlayer].GetRemainingTime() == 0 || timeCounter[Player.WhitePlayer].GetRemainingTime() == 0))
            {
                //One player is out of time
                scores = timeCounter[Player.BlackPlayer].GetRemainingTime() == 0 ? new Tuple<int, int>(0, maxScore) : new Tuple<int, int>(maxScore, 0);
            }
            else
            {
                // Get nb token per player
                (int black, int white) = (gameState.GetNbToken(GameBoard.CellState.BLACK), gameState.GetNbToken(GameBoard.CellState.WHITE));

                if (!GameEnded || gameState.GetNbToken(GameBoard.CellState.EMPTY) == 0)
                {
                    // Default Count number of token for each player
                    scores = new Tuple<int, int>(black, white);
                }
                else if (black == 0 || white == 0)
                {
                    //Eradication of a player
                    scores = black == 0 ? new Tuple<int, int>(0, maxScore) : new Tuple<int, int>(maxScore, 0);
                }
                else
                {
                    // No player can move
                    scores = black > white ? new Tuple<int, int>(maxScore - white, white) : new Tuple<int, int>(black, maxScore - black);
                }
            }
        }

        /// <summary>
        /// Allow to export data
        /// </summary>
        /// <returns>The game state</returns>
        public GameState Export()
        {
            return Export(indexState);
        }

        private GameState Export(int index)
        {
            GameBoard.CellState[,] gameboard = listGameBoard[index].Board;
            int[,] board = new int[gameboard.GetLength(0), gameboard.GetLength(1)];

            for (int i = 0; i < gameboard.GetLength(0); ++i)
            {
                for (int j = 0; j < gameboard.GetLength(1); ++j)
                {
                    board[i, j] = (int)gameboard[i, j];
                }
            }

            List<Tuple<char, int>> possiblesMoves = listGameBoard[index].PossibleMoves(PlayerToCellState(CurrentPlayerTurn));
            Tuple<long, long> remainingTimes = new Tuple<long, long>(timeCounter[Player.BlackPlayer].GetRemainingTime(), timeCounter[Player.WhitePlayer].GetRemainingTime());

            return new GameState(GameEnded, (int)CurrentPlayerTurn, scores, board, possiblesMoves, remainingTimes, (int)winner);
        }

        public ExportedGame Save()
        {
            var listGameState = new List<GameState>(indexState);
            for(int i = indexState; i >= 0; i--)
            {
                listGameState.Add(Export(i));
            }

            return new ExportedGame(BattleType, listGameState, CurrentPlayerTurn);
        }
    }
}
