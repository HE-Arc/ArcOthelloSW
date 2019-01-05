using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloMillenniumServer
{

    class GameManager
    {
        public int indexState { get; private set; }
        public List<GameState> listGameState;

        public event EventHandler<GameManagerArgs> OnGameFinished;

        public GameManager()
        {
        
        }

        public class GameManagerArgs
        {
            public string Winner { get; private set;  }

            public DateTime TotalTime { get; private set; }

            public Tuple<int, int> Score { get; private set; }

        }
    }
}
