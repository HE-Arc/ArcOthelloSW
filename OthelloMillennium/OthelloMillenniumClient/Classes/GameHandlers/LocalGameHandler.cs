using Tools;
using Tools.Classes;
using GameHandler = OthelloMillenniumClient.Classes.GameHandler;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Classe wrapper entre l'interface de jeu et la partie communication avec le serveur
    /// </summary>
    public class LocalGameHandler : GameHandler
    {
        public LocalGameHandler(BattleType battleType)
            : base(battleType)
        { }

        public override void Init(ExportedGame data)
        {
            return;
        }

        /// <summary>
        /// Instanciate two clients
        /// <para/>Send a search request
        /// </summary>
        public override void Init(string clientName, string opponentName)
        {
            switch (this.BattleType)
            {
                case BattleType.AgainstAI:
                    Client = new Client(PlayerType.Human, clientName);
                    Opponent = new Client(PlayerType.AI, opponentName);

                    Client.OnBeginReceived += Client_OnBeginReceived;
                    Client.OnAwaitReceived += Client_OnAwaitReceived;
                    Client.OnGameStateReceived += Client_OnGameStateReceived;
                    Opponent.OnGameStateReceived += Client_OnGameStateReceived;

                    // Send registers
                    Client.Register(GameType.Local);
                    Opponent.Register(GameType.Local);

                    // Send orders
                    Client.Search(BattleType.AgainstAI);
                    Opponent.Search(BattleType.AgainstPlayer);

                    break;
                case BattleType.AgainstPlayer:
                    Client = new Client(PlayerType.Human, clientName);
                    Opponent = new Client(PlayerType.Human, opponentName);

                    Client.OnBeginReceived += Client_OnBeginReceived;
                    Client.OnAwaitReceived += Client_OnAwaitReceived;
                    Client.OnGameStateReceived += Client_OnGameStateReceived;

                    // Send registers
                    Client.Register(GameType.Local);
                    Opponent.Register(GameType.Local);

                    // Send orders
                    Client.Search(BattleType.AgainstPlayer);
                    Opponent.Search(BattleType.AgainstPlayer);

                    break;
            }
        }

        protected override void Client_OnOpponentFound(object sender, OthelloTCPClientArgs e)
        {
        }
    }
}
