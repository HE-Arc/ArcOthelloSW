using System;
using System.Threading;

namespace Tools
{
    /// <summary>
    /// Wrapper of OthelloTCPClient
    /// </summary>
    public class Client : OthelloTCPClient
    {
        public event EventHandler<OthelloTCPClientArgs> OnOpponentAvatarChanged;
        public event EventHandler<OthelloTCPClientArgs> OnGameStartedReceived;
        public event EventHandler<OthelloTCPClientArgs> OnGameReadyReceived;

        #region Attributes
        private int avatarID;

        // Locked semaphore, will be released when registerSuccessful will be received
        private Semaphore semaphoreSearch = new Semaphore(0, 1);
        private Semaphore semaphoreReady = new Semaphore(0, 1);

        #endregion

        #region Properties

        public bool CanPlay { get; private set; }

        /// <summary>
        /// Get : get the Name binded
        /// Set : set the Name and inform the server of the change
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get : get the PlayerType
        /// Set : set the PlayerType
        /// </summary>
        public PlayerType PlayerType { get; set; }

        /// <summary>
        /// Get : get the Color
        /// Set : set the Color
        /// </summary>
        public Color Color { get; set; }

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
        }

        private void Client_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            switch (e.Order)
            {
                case OpponentAvatarChangedOrder order:
                    OnOpponentAvatarChanged?.Invoke(this, e);
                    break;

                case RegisterSuccessfulOrder order:
                    semaphoreSearch.Release();
                    break;

                case GameStartedOrder order:
                    OnGameStartedReceived?.Invoke(this, e);
                    break;

                case GameReadyOrder order:
                    OnGameReadyReceived?.Invoke(this, e);
                    semaphoreReady.Release();
                    break;
            }
        }

        /// <summary>
        /// Connect to a server
        /// </summary>
        /// <param name="gameType"></param>
        public void Register(GameType gameType)
        {
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
            // Wait for the client to be registred
            semaphoreSearch.WaitOne();

            // Send a request
            Send(new SearchOrder(battleType == BattleType.AgainstAI ? PlayerType.AI : PlayerType.Human));
        }

        /// <summary>
        /// Tell the gameHandler that the client is ready
        /// </summary>
        public void Ready()
        {
            // Wait for the game to be ready
            semaphoreReady.WaitOne();

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
