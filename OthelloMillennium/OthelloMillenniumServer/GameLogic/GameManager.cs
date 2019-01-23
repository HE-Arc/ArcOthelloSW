using OthelloMillenniumServer.GameLogic;
using System;
using System.Collections.Generic;
using Tools;

namespace OthelloMillenniumServer
{
    public class GameManager
    {
        #region Internal Classes
        
        private static int PlayerToCellState(Color Color)
        {
            return Color == Color.Black ? GameBoard.CellState.BLACK : GameBoard.CellState.WHITE;
        }

        private static Color CellStateToPlayer(int player)
        {
            return player == GameBoard.CellState.BLACK ? Color.Black : Color.White;
        }

        #endregion

        #region Properties
        public bool GameEnded { get; private set; }
        public GameType GameType { get; private set; }
        public Color CurrentPlayerTurn { get; private set; }

        #endregion

        #region Attributes
        private int indexState;
        private List<GameBoard> listGameBoard;
        private readonly Dictionary<Color, ITimer> timeCounter;
        private Tuple<int, int> scores;
        private Color winner;

        #endregion

        #region Events
        public event EventHandler<GameState> OnGameFinished;

        #endregion

        public GameManager()
        {
            GameType = TCPServer.Instance.Running ? TCPServer.Instance.Environnement : GameType.Online;

            //Init GameState
            indexState = 0;
            listGameBoard = new List<GameBoard>
            {
                GameBoard.CreateStartState()
            };

            timeCounter = new Dictionary<Color, ITimer>();
            if (GameType == GameType.Online)
            {
                timeCounter.Add(Color.Black, new StoppableTimer(Settings.TimePerPlayer));
                timeCounter.Add(Color.White, new StoppableTimer(Settings.TimePerPlayer));
            }
            else
            {
                timeCounter.Add(Color.Black, new StoppableCounter(Settings.TimePerPlayer));
                timeCounter.Add(Color.White, new StoppableCounter(Settings.TimePerPlayer));
            }

            CurrentPlayerTurn = Color.Black;

            ComputeScore();
        }

        public void Load(List<GameState> game)
        {
            if (GameEnded)
            {
                throw new Exception("Game ended");
            }
            else if (indexState > 0)
            {
                throw new Exception("Can't load during a game");
            }
            else
            {
                // Reset
                listGameBoard.Clear();
                indexState = 0;

                // Start loading
                foreach (GameState step in game)
                {
                    listGameBoard.Add(new GameBoard(step.Gameboard, step.PlayerTurn));
                    CurrentPlayerTurn = CellStateToPlayer(step.PlayerTurn);
                    scores = step.Scores;
                    winner = CellStateToPlayer(step.Winner);

                    if (GameType == GameType.Online)
                    {
                        timeCounter[Color.Black] = new StoppableTimer(step.RemainingTimes.Item1);
                        timeCounter[Color.White] = new StoppableTimer(step.RemainingTimes.Item2);
                    }
                    else
                    {
                        timeCounter[Color.Black] = new StoppableCounter(step.RemainingTimes.Item1);
                        timeCounter[Color.White] = new StoppableCounter(step.RemainingTimes.Item2);
                    }                   
                }

                // Current state is the last one
                indexState = listGameBoard.Count - 1;
            }
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

            // We start the counter
            timeCounter[CurrentPlayerTurn].Start();
        }

        /// <summary>
        /// Allow to play a move
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="isPlayerOne"></param>
        public void PlayMove(Tuple<char, int> coord, Color Color)
        {
            if (GameEnded)
            {
                throw new Exception("Game ended");
            }

            if (Color != CurrentPlayerTurn)
            {
                throw new Exception("Invalid Color turn");
            }
            
            timeCounter[CurrentPlayerTurn].Stop();
            
            int cellStatePlayer = PlayerToCellState(CurrentPlayerTurn);
            listGameBoard[indexState].ValidateMove(coord, cellStatePlayer);

            //Manage the case when we have made a moveback
            if (indexState + 1 < listGameBoard.Count)
            {
                listGameBoard.RemoveRange(indexState+1, listGameBoard.Count-indexState-1);
            }

            listGameBoard.Add(listGameBoard[indexState].ApplyMove(coord, cellStatePlayer));
            ++indexState;

            SwitchPlayer();

            timeCounter[CurrentPlayerTurn].Start();
            

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

            if (GameType == GameType.Online)
            {
                throw new Exception("Action not allowed in online game");
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

            if (GameType == GameType.Online)
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
        /// Switch the current Color
        /// </summary>
        private void SwitchPlayer()
        {
            int lastPlayer = listGameBoard[indexState].LastPlayer;
            int nextPlayer = (lastPlayer % 2 + 1);
            CurrentPlayerTurn = (Color) (listGameBoard[indexState].PlayerCanPlay(nextPlayer) ? nextPlayer : lastPlayer);
        }

        /// <summary>
        /// End a game
        /// </summary>
        private void EndGame()
        {
            GameEnded = true;
            
            timeCounter[CurrentPlayerTurn].Stop();
            ComputeScore();
            winner = scores.Item1 > scores.Item2 ? Color.Black : Color.White;

            EventHandler<GameState> handler = OnGameFinished;
            handler(this, Export());
        }

        private void ComputeScore()
        {
            GameBoard gameState = listGameBoard[indexState];
            int maxScore = gameState.Board.GetLength(0) * gameState.Board.GetLength(1);
            
            if (GameType == GameType.Online && (timeCounter[Color.Black].GetRemainingTime() == 0 || timeCounter[Color.White].GetRemainingTime() == 0))
            {
                //One Color is out of time
                scores = timeCounter[Color.Black].GetRemainingTime() == 0 ? new Tuple<int, int>(0, maxScore) : new Tuple<int, int>(maxScore, 0);
            }
            else
            {
                // Get nb token per Color
                int black = gameState.GetNbToken(GameBoard.CellState.BLACK);
                int white = gameState.GetNbToken(GameBoard.CellState.WHITE);

                if (!GameEnded || gameState.GetNbToken(GameBoard.CellState.EMPTY) == 0)
                {
                    // Default Count number of token for each Color
                    scores = new Tuple<int, int>(black, white);
                }
                else if (black == 0 || white == 0)
                {
                    //Eradication of a Color
                    scores = black == 0 ? new Tuple<int, int>(0, maxScore) : new Tuple<int, int>(maxScore, 0);
                }
                else
                {
                    // No Color can move
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
            int[,] gameboard = listGameBoard[index].Board;
            int[,] board = new int[gameboard.GetLength(0), gameboard.GetLength(1)];

            for (int i = 0; i < gameboard.GetLength(0); ++i)
            {
                for (int j = 0; j < gameboard.GetLength(1); ++j)
                {
                    board[i, j] = gameboard[i, j];
                }
            }

            List<Tuple<char, int>> possiblesMoves = listGameBoard[index].PossibleMoves(PlayerToCellState(CurrentPlayerTurn));
            Tuple<long, long> remainingTimes = new Tuple<long, long>(timeCounter[Color.Black].GetRemainingTime(), timeCounter[Color.White].GetRemainingTime());

            return new GameState(GameEnded, (int)CurrentPlayerTurn, scores, board, possiblesMoves, remainingTimes, (int)winner, listGameBoard.Count - 1, indexState);
        }

        public List<GameState> Save()
        {
            var listGameState = new List<GameState>(indexState);
            for(int i = indexState; i >= 0; i--)
            {
                listGameState.Add(Export(i));
            }

            return listGameState;
        }
    }
}
