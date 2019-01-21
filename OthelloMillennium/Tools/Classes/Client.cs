using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// Wrapper of OthelloTCPClient
    /// </summary>
    public class Client : OthelloTCPClient
    {
        public event EventHandler<OthelloTCPClientArgs> OnRegisterSuccessful;
        public event EventHandler<OthelloTCPClientArgs> OnOpponentAvatarChanged;
        public event EventHandler<OthelloTCPClientArgs> OnGameStartedReceived;
        public event EventHandler<OthelloTCPClientArgs> OnGameReadyReceived;

        #region Attributes

        private string name;
        private PlayerType playerType;
        private Color color;
        private int avatarID;

        // Locked semaphore
        private Semaphore semaphoreSearch = new Semaphore(0, 1);

        #endregion

        #region Properties

        public bool CanPlay { get; private set; }

        /// <summary>
        /// Get : get the Name binded
        /// Set : set the Name and inform the server of the change
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Get : get the PlayerType
        /// Set : set the PlayerType
        /// </summary>
        public PlayerType PlayerType
        {
            get
            {
                return playerType;
            }
            set
            {
                playerType = value;
            }
        }

        /// <summary>
        /// Get : get the Color
        /// Set : set the Color
        /// </summary>
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        /// <summary>
        /// Get : get the AvatarID binded
        /// Set : set the AvatarID and inform the server of the change
        /// </summary>
        public int AvatarID {
            get
            {
                return avatarID;
            }
            set
            {
                avatarID = value;
                Send(new AvatarChangedOrder(avatarID));
            }
        }

        #endregion

        public Client(PlayerType type, string name)
            : base()
        {
            PlayerType = type;
            Name = name;

            // Respond to order
            OnOrderReceived += Client_OnOrderReceived;
            OnRegisterSuccessful += Client_OnRegisterSuccessful;
        }

        private void Client_OnRegisterSuccessful(object sender, OthelloTCPClientArgs e)
        {
            semaphoreSearch.Release();
        }

        private void Client_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            switch (e.Order)
            {
                case OpponentAvatarChangedOrder order:
                    OnOpponentAvatarChanged?.Invoke(this, e);
                    break;

                case RegisterSuccessfulOrder order:
                    OnRegisterSuccessful?.Invoke(this, e);
                    break;

                // Used by the server
                case AvatarChangedOrder order:
                    avatarID = order.AvatarID;
                    break;

                case GameStartedOrder order:
                    OnGameStartedReceived?.Invoke(this, e);
                    break;

                case GameReadyOrder order:
                    OnGameReadyReceived?.Invoke(this, e);
                    break;
            }

            // TODO REMOVE
            Console.Error.WriteLine(e.Order.GetAcronym());
        }

        /// <summary>
        /// Connect to a server
        /// </summary>
        /// <param name="gameType"></param>
        public void Register(GameType gameType)
        {
            // If connected close the connection
            TcpClient?.Close();

            switch (gameType)
            {
                case GameType.Local:
                    ConnectTo(Properties.Settings.Default.LocalHostname, Properties.Settings.Default.LocalPort);
                    break;
                case GameType.Online:
                    ConnectTo(Properties.Settings.Default.OnlineHostname, Properties.Settings.Default.OnlinePort);
                    break;
                default:
                    throw new Exception("Invalid gameType provided");
            }

            // Send a register request
            Send(new RegisterRequestOrder(PlayerType, Name));
        }

        /// <summary>
        /// Send a message in order to register itself
        /// </summary>
        /// <param name="battleType"></param>
        public void Search(BattleType battleType)
        {
            semaphoreSearch.WaitOne();

            // Send a request
            Send(new SearchOrder(battleType == BattleType.AgainstAI ? PlayerType.AI : PlayerType.Human));
        }

        /// <summary>
        /// Tell the gameHandler that the client is ready
        /// </summary>
        public void Ready()
        {
            Send(new ReadyOrder());
        }

        /// <summary>
        /// Sends to the server the location where the new token has been placed
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="column">column</param>
        public void Play(char row, int column)
        {
            if (CanPlay)
                Send(new PlayMoveOrder(new Tuple<char, int>(row, column)));
            else
                throw new Exception("Not allowed to play !");
        }
    }
}
