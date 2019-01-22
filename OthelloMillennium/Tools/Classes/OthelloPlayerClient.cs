using System;

namespace Tools
{
    public class OthelloPlayerClient : IOrderHandler
    {
        // properties

        #region Attributes
        private int avatarId;

        private Client client;
        private IOrderHandler orderHandler;

        #endregion

        #region Properties

        /// <summary>
        /// Get : get the Name binded
        /// Set : set the Name and inform the server of the change
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get : get the PlayerType
        /// Set : set the PlayerType
        /// </summary>
        public PlayerType PlayerType { get; private set; }

        /// <summary>
        /// Get : get the Color
        /// Set : set the Color
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Get : get the AvatarID binded
        /// Set : set the AvatarID and inform the server of the change
        /// </summary>
        public int AvatarID
        {
            get
            {
                return avatarId;
            }
            set
            {
                if (!IsLocalPlayer && PlayerState.LOBBY_CHOICE != PlayerState)
                {
                    throw new Exception("This is a remote player you cannot edit it");
                }
                avatarId = value;
                client.Send(new AvatarChangedOrder(avatarId));
            }
        }

        public bool IsLocalPlayer { get; private set; }

        public PlayerState PlayerState { get; private set; }
        #endregion

        public OthelloPlayerClient(PlayerType playerType, string name)
        {
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentException("name can't be null or empty");
            PlayerType = playerType;
            
            // Set the set to initial
            PlayerState = PlayerState.INITIAL;
        }

        public void Connect(GameType localOrOnline)
        {
            // Init a new client
            client = new Client();

            // Connect the client to the target gametype server
            if(localOrOnline == GameType.Local)
            {
                IsLocalPlayer = true;
                client.ConnectTo(Properties.Settings.Default.LocalHostname, Properties.Settings.Default.LocalPort);
            }
            else
            {
                IsLocalPlayer = false;
                client.ConnectTo(Properties.Settings.Default.OnlineHostname, Properties.Settings.Default.OnlinePort);
            }
            
            // Attach the client OrderHandler to this
            client.SetOrderhandler(this);
        }

        public void SetOrderhandler(IOrderHandler orderHandler)
        {
            this.orderHandler = orderHandler;
        }

        public void Register()
        {
            if (PlayerState != PlayerState.INITIAL) throw new Exception("Action not available");
            client.Send(new RegisterRequestOrder(PlayerType, Name));
            PlayerState = PlayerState.REGISTERING;
        }

        public void SearchOpponent(PlayerType playerType)
        {
            if (PlayerState != PlayerState.REGISTERED) throw new Exception("Action not available");
            client.Send(new SearchOrder(playerType));

            // Switch client state to searching
            PlayerState = PlayerState.SEARCHING;
        }

        public void ReadyToPlay()
        {
            if (PlayerState != PlayerState.LOBBY_CHOICE) throw new Exception("Action not allowed");
            client.Send(new ReadyOrder());

            // Switch client state to ready
            PlayerState = PlayerState.READY;
        }

        public void Play(char row, int column)
        {
            if (PlayerState != PlayerState.MY_TURN) throw new Exception("Action not allowed");
            client.Send(new PlayMoveOrder(new Tuple<char, int>(row, column)));
        }

        public void HandleOrder(Order orderHandled)
        {
            if (orderHandler == null)
            {
                Console.Error.WriteLine("WARNING ! - [OthelloPlayerClient.handleOrder] handleOrder should normally be set!");
            }

            switch (orderHandled)
            {
                #region Forwarded orders
                case RegisterSuccessfulOrder order:
                    PlayerState = PlayerState.REGISTERED;
                    orderHandler?.HandleOrder(order);
                    break;

                case OpponentFoundOrder order:
                    PlayerState = PlayerState.LOBBY_CHOICE;
                    orderHandler?.HandleOrder(order);
                    break;

                case GameReadyOrder order:
                    PlayerState = PlayerState.ABOUT_TO_START;
                    orderHandler?.HandleOrder(order);
                    break;

                case GameStartedOrder order:
                    PlayerState = Color == Color.Black ? PlayerState.MY_TURN : PlayerState.OPPONENT_TURN;
                    orderHandler?.HandleOrder(order);
                    break;
                
                case OpponentAvatarChangedOrder order:
                    orderHandler?.HandleOrder(order);
                    break;
                #endregion

                #region Properties received by the server
                case AssignColorOrder order:
                    Color = (Color)order.Color;
                    break;

                case AssignAvatarIDOrder order:
                    // DO NOT change the property or it will send an order
                    avatarId = order.AvatarID;
                    break;
                #endregion
            }
        }
    }
}
