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
        public event EventHandler<OthelloTCPClientArgs> OnOpponentDataChanged;
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
                Synchronize();
            }
        }

        /// <summary>
        /// Get : get the PlayerType binded
        /// Set : set the PlayerType and inform the server of the change
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
                Synchronize();
            }
        }

        /// <summary>
        /// Get : get the Color binded
        /// Set : set the Color and inform the server of the change
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
                Synchronize();
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
                Synchronize();
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
            OnDataReceived += Client_OnDataReceived;
            OnRegisterSuccessful += Client_OnRegisterSuccessful;
        }

        private void Client_OnRegisterSuccessful(object sender, OthelloTCPClientArgs e)
        {
            semaphoreSearch.Release();
        }

        private void Client_OnDataReceived(object sender, OthelloTCPClientDataArgs e)
        {
            AvatarID = e.Data.AvatarID;
            Color = (Color)e.Data.Color;

            // Inform opponent that this client's data has changed
            Send(new OpponentDataChangedOrder(e.Data));
        }

        private void Client_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            switch (e.Order)
            {
                case GetDataOrder order:
                    Send(GenerateData());
                    break;

                case RegisterSuccessfulOrder order:
                    OnRegisterSuccessful?.Invoke(this, e);
                    break;

                case OpponentDataChangedOrder order:
                    OnOpponentDataChanged?.Invoke(this, e);
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
        /// Whenever a property is changed this method is called
        /// </summary>
        private void Synchronize()
        {
            if (TcpClient != null && TcpClient.Connected)
            {
                // TODO FIX ENUM
                Send(GenerateData());
            }
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
            Send(new RegisterOrder(PlayerType, Name));
        }

        /// <summary>
        /// Send a message in order to register itself
        /// </summary>
        /// <param name="battleType"></param>
        public void Search(BattleType battleType)
        {
            if (TcpClient.Connected)
            {
                semaphoreSearch.WaitOne();

                // Send a request
                Send(new SearchOrder(battleType == BattleType.AgainstAI ? PlayerType.AI : PlayerType.Human));
            }
        }

        /// <summary>
        /// Tell the gameHandler that the client is ready
        /// </summary>
        public void Ready()
        {
            //TODO SGAN Check
            if (TcpClient.Connected)
            {
                Send(new ReadyOrder());
            }
        }

        /// <summary>
        /// Generate a data package
        /// </summary>
        /// <returns>returns it</returns>
        public Data GenerateData()
        {
            return new Data(PlayerType, Color, Name, AvatarID);
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
