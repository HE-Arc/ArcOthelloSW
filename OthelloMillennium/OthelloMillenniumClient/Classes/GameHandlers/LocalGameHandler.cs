using OthelloMillenniumClient.Classes;
using Tools.Classes;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Classe wrapper entre l'interface de jeu et la partie communication avec le serveur
    /// </summary>
    public class LocalGameHandler : IGameHandler
    {
        #region Properties
        public GameType GameType { get; } = GameType.Local;
        public BattleType BattleType { get; private set; }
        #endregion

        #region Attributes

        #endregion

        public LocalGameHandler(BattleType battleType)
        {
            BattleType = battleType;
        }

        public Client GetClient()
        {
            throw new System.NotImplementedException();
        }

        public Client GetCurrentPlayer()
        {
            throw new System.NotImplementedException();
        }

        public Client GetOpponent()
        {
            throw new System.NotImplementedException();
        }

        public void StartNewGame(BattleType battleType)
        {
            //TODO
        }

    }
}
