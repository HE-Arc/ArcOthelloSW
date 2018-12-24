using System;
using System.Collections.Generic;

namespace OthelloMillenniumServer
{

    class GameManager
    {
        #region Internal Classes
        public class GameManagerArgs
        {
            public string Winner { get; private set; }

            public DateTime TotalTime { get; private set; }

            public Tuple<int, int> Score { get; private set; }
        }

        public enum GameType
        {
            SinglePlayer = 0,
            MultiPlayer = 1
        }

        #endregion

        #region Attributes
        private int indexState;
        private GameType gameType;
        private List<GameState> listGameState;
        
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

            this.gameType = gameType;
        }

        public void PlayMove((char, int) coord, bool isPlayerOne)
        {
            GameState.CellState cellStatePlayer = isPlayerOne ? GameState.CellState.BLACK : GameState.CellState.WHITE;
            listGameState[indexState].ValidMove(coord, cellStatePlayer);

            //Manage case when we have made a moveback
            if(indexState + 1 < listGameState.Count)
            {
                listGameState.RemoveRange(indexState+1, listGameState.Count-indexState-1);
            }

            listGameState.Add(listGameState[indexState].ApplyMove(coord, cellStatePlayer));
            ++indexState;
        }

        /// <summary>
        /// Reverse a move
        /// </summary>
        public void moveBack()
        {
            if(gameType == GameType.MultiPlayer)
            {
                throw new Exception("Action not allowed in Multiplayer game type");
            }

            if (indexState > 0)
            {
                --indexState;
            }
            //TODO SEGAN, Veux-tu une exception si on ne peux pas revenir plus en arrière
        }
        
        /// <summary>
        /// Cancel a move back
        /// </summary>
        public void moveForward()
        {
            if (gameType == GameType.MultiPlayer)
            {
                throw new Exception("Action not allowed in Multiplayer game type");
            }

            if (indexState < listGameState.Count - 1)
            {
                ++indexState;
            }
            //TODO SEGAN, Veux-tu une exception si on est déjà au dernier état
        }
    }
}
