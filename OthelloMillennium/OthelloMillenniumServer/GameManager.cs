using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloMillenniumServer
{

    class GameManager
    {
        private int indexState;
        private GameType gameType;
        private List<GameState> listGameState = new List<GameState>();

        public event EventHandler<GameManagerArgs> OnGameFinished;

        public GameManager(GameType gameType)
        {
            //Init GameState
            indexState = 0;
            listGameState.Add(GameState.CreateStartState());

            this.gameType = gameType;
        }

        public void PlayMove(Tuple<char, int> coord, bool isPlayerOne)
        {
            listGameState[indexState].ValidMove(coord);

            //Manage case when we have made a moveback
            if(indexState + 1 < listGameState.Count)
            {
                listGameState.RemoveRange(indexState+1, listGameState.Count-indexState-1);
            }

            listGameState.Add(listGameState[indexState].ApplyMove(coord, isPlayerOne ? CellState.BLACK : CellState.WHITE));
            ++indexState;
        }

        public void moveBack()
        {
            if(gameType == GameType.MultiPlayer)
            {
                throw new Exception("Action not allowed in Multiplayer game type");
            }

            if (indexState > 0)
                --indexState;
        }
        
        public void moveForward()
        {
            if (gameType == GameType.MultiPlayer)
            {
                throw new Exception("Action not allowed in Multiplayer game type");
            }

            if (indexState < listGameState.Count-1)
                ++indexState;
        }

        public class GameManagerArgs
        {
            public string Winner { get; private set;  }

            public DateTime TotalTime { get; private set; }

            public Tuple<int, int> Score { get; private set; }
        }
    }
}
