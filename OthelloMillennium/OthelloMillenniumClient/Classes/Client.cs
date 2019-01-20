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
        private event EventHandler<OthelloTCPClientArgs> OnOpponentFoundReceived;
        public event EventHandler<OthelloTCPClientArgs> OnOpponentAvatarChanged;
        public event EventHandler<OthelloTCPClientArgs> OnGameStartedReceived;

        public PlayerType PlayerType { get; private set; }
        public string Name { get; private set; }
        public int AvatarID { get; private set; }
        public bool CanPlay { get; private set; }

        /// <summary>
        /// Try to retrive opponent name
        /// </summary>
        public string OpponentName => Properties["OpponentName"] as string;

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
                    CanPlay = false;
                    break;
                case PlayerBeginOrder order:
                    CanPlay = true;
                    break;
                case OpponentFoundOrder order:
                    this.Properties.Add("OpponentName", (e.Order as OpponentFoundOrder).Opponent.Properties["Name"]);
                    OnOpponentFoundReceived?.Invoke(this, e);
                    break;
                case AvatarChangedOrder order:
                    OnOpponentAvatarChanged?.Invoke(this, e);
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

        public void ChangeAvatar(int avatarID)
        {
            AvatarID = avatarID;
            this.Send(new AvatarChangedOrder(avatarID));
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
            if (CanPlay)
                this.Send(new PlayMoveOrder(new Tuple<char, int>(row, column)));
            else
                throw new Exception("Not allowed to play !");
        }
    }
}
