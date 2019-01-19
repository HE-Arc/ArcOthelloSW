using OthelloMillenniumClient.Classes;
using OthelloMillenniumServer;
using Tools;
using Tools.Classes;
using GameHandler = OthelloMillenniumClient.Classes.GameHandler;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Classe wrapper entre l'interface de jeu et la partie communication avec le serveur
    /// </summary>
    public class OnlineGameHandler : GameHandler
    {
        public OnlineGameHandler(BattleType battleType)
            : base (battleType)
        { }

        public override void Init(ExportedGame data)
        {
            return;
        }

        /// <summary>
        /// Instanciate two clients
        /// <para/>Send a search request
        /// </summary>
        public override void Init(string clientName, string opponentName = "")
        {
            switch (BattleType)
            {
                case BattleType.AgainstAI:
                    Client = new Client(PlayerType.Human, clientName);

                    Client.OnBeginReceived += Client_OnBeginReceived;
                    Client.OnAwaitReceived += Client_OnAwaitReceived;
                    Client.OnGameStateReceived += Client_OnGameStateReceived;

                    // Register client
                    Client.Register(GameType.Online);

                    // Send orders
                    Client.Search(BattleType.AgainstAI);

                    break;
                case BattleType.AgainstPlayer:
                    Client = new Client(PlayerType.Human, clientName);

                    Client.OnBeginReceived += Client_OnBeginReceived;
                    Client.OnAwaitReceived += Client_OnAwaitReceived;
                    Client.OnGameStateReceived += Client_OnGameStateReceived;

                    // Register client
                    Client.Register(GameType.Online);

                    // Send orders
                    Client.Search(BattleType.AgainstPlayer);

                    break;
            }
        }

        protected override void Client_OnOpponentFound(object sender, OthelloTCPClientArgs e)
        {
            Opponent = sender as Client;
        }
    }
}
