using OthelloMillenniumClient.Classes;
using OthelloMillenniumServer;
using Tools;
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

        public Client Client { get; private set; }
        public Client Opponent { get; private set; }
        public GameState GameState { get; private set; }
        #endregion

        #region Attributes
        private bool isClientTurn;
        #endregion

        public LocalGameHandler(BattleType battleType)
        {
            BattleType = battleType;
        }

        public Client GetCurrentPlayer()
        {
            return isClientTurn ? Client : Opponent;
        }

        public void StartNewGame()
        {
            switch (this.BattleType)
            {
                case BattleType.AgainstAI:
                    Client = new Client(PlayerType.Human, GameType.Local);
                    Opponent = new Client(PlayerType.AI, GameType.Local);

                    Client.OnBeginReceived += Client_OnBeginReceived;
                    Client.OnAwaitReceived += Client_OnAwaitReceived;
                    Client.OnGameStateReceived += Client_OnGameStateReceived;
                    Opponent.OnGameStateReceived += Client_OnGameStateReceived;

                    // Send orders
                    Client.Search(PlayerType.AI);
                    Opponent.Search(PlayerType.Human);

                    break;
                case BattleType.AgainstPlayer:
                    Client = new Client(PlayerType.Human, GameType.Local);
                    Opponent = new Client(PlayerType.Human, GameType.Local);

                    Client.OnBeginReceived += Client_OnBeginReceived;
                    Client.OnAwaitReceived += Client_OnAwaitReceived;
                    Client.OnGameStateReceived += Client_OnGameStateReceived;

                    // Send orders
                    Client.Search(PlayerType.Human);
                    Opponent.Search(PlayerType.Human);

                    break;
            }
        }

        private void Client_OnBeginReceived(object sender, OthelloTCPClientArgs e)
        {
            isClientTurn = true;
        }

        private void Client_OnAwaitReceived(object sender, OthelloTCPClientArgs e)
        {
            isClientTurn = false;
        }

        private void Client_OnGameStateReceived(object sender, OthelloTCPClientArgs e)
        {
            GameState = e.GameState;
        }
    }
}
