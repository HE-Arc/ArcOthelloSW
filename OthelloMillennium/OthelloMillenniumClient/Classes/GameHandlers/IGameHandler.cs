using Tools.Classes;

namespace OthelloMillenniumClient.Classes
{
    public interface IGameHandler
    {
        Client GetClient();
        Client GetOpponent();
        Client GetCurrentPlayer();
        void StartNewGame(BattleType type);
    }
}
