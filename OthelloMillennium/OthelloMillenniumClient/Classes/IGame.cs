using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace OthelloMillenniumClient
{
    public interface IGame
    {
        void OnGameStartServer();
        void OnGameStateUpdateServer(GameState gameState);
        void OnGameEndedServer();
    }
}
