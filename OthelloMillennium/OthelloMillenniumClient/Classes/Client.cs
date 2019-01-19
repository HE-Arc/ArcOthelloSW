using OthelloMillenniumServer;
using System;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Wrapper of OthelloTCPClient
    /// <para/>Client must register itself and then search a game
    /// </summary>
    public class Client : OthelloTCPClient
    {
        public event EventHandler<OthelloTCPClientArgs> OnBeginReceived;
        public event EventHandler<OthelloTCPClientArgs> OnAwaitReceived;
        public event EventHandler<OthelloTCPClientArgs> OnOpponentFoundReceived;
        public event EventHandler<OthelloTCPClientArgs> OnGameStartedReceived;

        public PlayerType PlayerType { get; private set; }
        public string Name { get; private set; }

        public Client(PlayerType type, string name)
            : base()
        {
            PlayerType = type;
            Name = name;

            this.OnOrderReceived += Client_OnOrderReceived;
        }

        private void Client_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            switch(e.Order)
            {
                case PlayerAwaitOrder order:
                    OnAwaitReceived?.Invoke(this, e);
                    break;
                case PlayerBeginOrder order:
                    OnBeginReceived?.Invoke(this, e);
                    break;
                case OpponentFoundOrder order:
                    OnOpponentFoundReceived?.Invoke(this, e);
                    break;
                case StartOfTheGameOrder order:
                    OnGameStartedReceived?.Invoke(this, e);
                    break;
            }
        }

        /// <summary>
        /// Connect to a server
        /// </summary>
        /// <param name="gameType"></param>
        public void Register(GameType gameType)
        {
            if (this.TcpClient.Connected)
            {
                this.TcpClient.Close();
            }

            switch (gameType)
            {
                case GameType.Local:
                    this.ConnectTo("localhost", TCPServer.Port);
                    break;
                case GameType.Online:
                    this.ConnectTo(OthelloMillenniumClient.Properties.Settings.Default.OnlineHostname, OthelloMillenniumClient.Properties.Settings.Default.OnlinePort);
                    break;
                default:
                    throw new Exception("Invalid gameType provided");
            }

            // Send a register request
            this.Send(new RegisterOrder(PlayerType, Name));
        }

        /// <summary>
        /// Send a message in order to register itself
        /// </summary>
        /// <param name="battleType"></param>
        public void Search(BattleType battleType)
        {
            if (this.TcpClient.Connected)
            {
                // Send a request
                this.Send(new SearchOrder(battleType == BattleType.AgainstAI ? PlayerType.AI : PlayerType.Human));
            }
        }

        /// <summary>
        /// Sends to the server the location where the new token has been placed
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="column">column</param>
        public void Play(char row, int column)
        {
            this.Send(new PlayMoveOrder(new Tuple<char, int>(row, column)));
        }
    }
}
