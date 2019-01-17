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

        public override void Init()
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

        protected override void Client_OnOpponentFound(object sender, OthelloTCPClientArgs e)
        {
        }
    }
}
