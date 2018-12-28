using OthelloMillenniumServer.GameLogic;
using System;
using System.Collections.Generic;

namespace OthelloMillenniumServer
{
    public class GameManager
    {
        #region Internal Classes
        public class GameManagerArgs
        {
            public string Winner { get; private set; }

            public (DateTime, DateTime) TotalTime { get; private set; }

            public (int, int) Score { get; private set; }
        }

        public enum GameType
        {
            SinglePlayer = 0,
            MultiPlayer = 1
        }


        public enum Player
        {
            BlackPlayer = 0,
            WhitePlayer = 1
        }

        private static GameState.CellState PlayerToCellState(Player player)
        {
            return player == Player.BlackPlayer ? GameState.CellState.BLACK : GameState.CellState.WHITE;
        }

        #endregion

        #region Properties
        public bool GameEnded { get; private set; }

        #endregion

        #region Attributes
        private Player CurrentPlayerTurn;
        private int indexState;
        private GameType gameType;
        private List<GameState> listGameState;
        private Dictionary<Player, StoppableTimer> timeCounter;

        #endregion

        #region Events
        public event EventHandler<GameManagerArgs> OnGameFinished;

        #endregion

        public GameManager(GameType gameType)
        {
            //Init GameState
            indexState = 0;
            listGameState = new List<GameState>();
            listGameState.Add(GameState.CreateStartState());
            timeCounter = new Dictionary<Player, StoppableTimer>()
            {
                { Player.BlackPlayer, new StoppableTimer(Settings.TimePerPlayer) },
                { Player.WhitePlayer, new StoppableTimer(Settings.TimePerPlayer) }
            };

            CurrentPlayerTurn = Player.BlackPlayer;
            this.gameType = gameType;
        }

        /// <summary>
        /// Start the game counter
        /// </summary>
        public void Start()
        {
            //We start the counter
            if(gameType == GameType.MultiPlayer)
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
            if (player != CurrentPlayerTurn)
            {
                throw new Exception("Invalid player turn");
            }

            timeCounter[CurrentPlayerTurn].Stop();
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
            timeCounter[CurrentPlayerTurn].Start();

            //Manage end of the game
            if (listGameState[indexState].GameEnded)
            {
                EndGame();
            }
        }
        
        /// <summary>
        /// TODO REMOVE for test purposes
        /// </summary>
        /// <returns></returns>
        public GameState GetGameState()
        {
            return listGameState[indexState];
        }

        /// <summary>
        /// Reverse a move
        /// </summary>
        public void MoveBack()
        {
            if(gameType == GameType.MultiPlayer)
            {
                throw new Exception("Action not allowed in Multiplayer game type");
            }

            if (indexState > 0)
            {
                --indexState;
                SwitchPlayer();
            }
            //TODO SEGAN, Veux-tu une exception si on ne peux pas revenir plus en arrière
        }
        
        /// <summary>
        /// Cancel a move back
        /// </summary>
        public void MoveForward()
        {
            if (gameType == GameType.MultiPlayer)
            {
                throw new Exception("Action not allowed in Multiplayer game type");
            }

            if (indexState < listGameState.Count - 1)
            {
                ++indexState;
                SwitchPlayer();
            }
            //TODO SEGAN, Veux-tu une exception si on est déjà au dernier état
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimeout(Object source, EventArgs e)
        {
            //TODO Manage timeout of a player
            EndGame();
        }

        /// <summary>
        /// Switch the current player
        /// </summary>
        private void SwitchPlayer()
        {
            CurrentPlayerTurn = (Player)(((int)CurrentPlayerTurn + 1) % 2);
        }

        /// <summary>
        /// End a game
        /// </summary>
        private void EndGame()
        {
            GameEnded = true;
            EventHandler<GameManagerArgs> handler = OnGameFinished;
            GameManagerArgs gameManagerArgs = new GameManagerArgs();

            //TODO Complete event values
            //TODO Choose if we can go back in singleplayer mode

            handler(this, gameManagerArgs);
        }
    }
}
