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

        public override void Init()
        {
            switch (BattleType)
            {
                case BattleType.AgainstAI:
                    Client = new Client(PlayerType.Human, GameType.Online);

                    Client.OnBeginReceived += Client_OnBeginReceived;
                    Client.OnAwaitReceived += Client_OnAwaitReceived;
                    Client.OnGameStateReceived += Client_OnGameStateReceived;

                    // Send orders
                    Client.Search(PlayerType.AI);

                    break;
                case BattleType.AgainstPlayer:
                    Client = new Client(PlayerType.Human, GameType.Online);

                    Client.OnBeginReceived += Client_OnBeginReceived;
                    Client.OnAwaitReceived += Client_OnAwaitReceived;
                    Client.OnGameStateReceived += Client_OnGameStateReceived;

                    // Send orders
                    Client.Search(PlayerType.Human);

                    break;
            }
        }

        protected override void Client_OnOpponentFound(object sender, OthelloTCPClientArgs e)
        {
            Opponent = sender as Client;
        }
    }
}
