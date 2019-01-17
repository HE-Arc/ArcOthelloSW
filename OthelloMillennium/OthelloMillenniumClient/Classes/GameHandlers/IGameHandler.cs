using Tools;
using Tools.Classes;

namespace OthelloMillenniumClient.Classes
{
    public interface IGameHandler
    {
        Client Client { get; }
        Client Opponent { get; }
        GameState GameState { get; }

        Client GetCurrentPlayer();
        void StartNewGame();
    }
}
