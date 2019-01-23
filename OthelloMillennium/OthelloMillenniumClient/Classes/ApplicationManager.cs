using OthelloMillenniumClient.Classes.GameHandlers;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Current project must have a reference to OthelloMillenniumServer.dll
    /// </para>Bind HMI events to TCP messages
    /// </summary>
    public class ApplicationManager : IOrderHandler
    {
        #region Singleton
        private static readonly object padlock = new object();
        private static ApplicationManager instance = null;

        private ConcurrentQueue<Order> orderReceived;
        private Task taskOrderHandler;

        private GameHandler gameHandler;
        public GameState GameState { get; private set; }

        public static ApplicationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        instance = new ApplicationManager();
                    }
                }
                return instance;
            }
        }

        #endregion

        public GameType GameType { get; private set; }
        public IHome Home { get; set; }
        public ILobby Lobby { get; set; }
        public IGame Game { get; set; }

        private ApplicationManager()
        {
            orderReceived = new ConcurrentQueue<Order>();

            taskOrderHandler = new Task(ManageOrder);
            taskOrderHandler.Start();
        }

        private void ManageOrder()
        {
            while (orderReceived == null)
            {
                Thread.Sleep(50);
            }

            while (true)
            {
                while (!orderReceived.IsEmpty)
                {
                    orderReceived.TryDequeue(out Order order);
                    ExecuteOrder(order);
                }
                Thread.Sleep(50);
            }
        }

        private void ExecuteOrder(Order handledOrder)
        {
            switch (handledOrder)
            {
                case RegisterSuccessfulOrder order:
                    //TODO Display registered
                    break;

                case OpponentFoundOrder order:
                    //TODO Opponent found
                    break;

                case GameReadyOrder order:
                    Home.OnLaunchLobbyServer();
                    break;

                case GameStartedOrder order:
                    GameState = (order as GameStartedOrder).InitialState;
                    Lobby.OnLaunchGameServer();
                    //Game.OnGameStartServer();
                    break;

                case OpponentAvatarChangedOrder order:
                    Lobby.OnUpdateOpponentColorServer(PlayersColor().Item2, order.AvatarID);
                    break;
                    
                case UpdateGameStateOrder order:
                    GameState = (order as UpdateGameStateOrder).GameState;
                    Game.OnGameStateUpdateServer(GameState);
                    break;

                case GameEndedOrder order:
                    Game.OnGameEndedServer();
                    break;

                case LoadResponseOrder order:
                    // TODO (NiceToHave) share id with friends order.GameID;
                    break;
            }
        }

        public void SetOrderHandler(IOrderHandler handler)
        {
            throw new NotImplementedException("[SetOrderHandler] : ApplicationManager");
        }
        
        public void JoinGameLocal(PlayerType playerTypeOne, string playerNameOne, PlayerType playerTypeTwo, string playerNameTwo)
        {
            OthelloPlayerClient player1 = new OthelloPlayerClient(playerTypeOne, playerNameOne);
            OthelloPlayerClient player2 = new OthelloPlayerClient(playerTypeTwo, playerNameTwo);

            JoinGameLocal(player1, player2);
        }

        public void JoinGameLocal(OthelloPlayerClient player1, OthelloPlayerClient player2)
        {
            GameType = GameType.Local;
            gameHandler = new LocalGameHandler();
            gameHandler.SetOrderHandler(this);

            (gameHandler as LocalGameHandler).JoinGame(player1, player2);
        }
        
        public void JoinGameOnline(PlayerType playerType, string playerName, BattleType battleType)
        {
            OthelloPlayerClient player = new OthelloPlayerClient(playerType, playerName);

            JoinGameOnline(player, battleType);
        }

        public void JoinGameOnline(OthelloPlayerClient player, BattleType battleType)
        {
            GameType = GameType.Online;
            gameHandler = new OnlineGameHandler();
            gameHandler.SetOrderHandler(this);

            (gameHandler as OnlineGameHandler).JoinGame(player, battleType);
        }

        public void LaunchGame() => gameHandler.LaunchGame();

        public void AvatarIdChange(Color color, int avatarId) => gameHandler.AvatarIdChange(color, avatarId);

        public PlayerDataExport GetPlayers() => gameHandler.GetPlayers();
        public Tuple<Color, Color> PlayersColor() => gameHandler.PlayersColor();
        public int PlayersAvatarId(Color color) => gameHandler.PlayersAvatarId(color);

        public void HandleOrder(IOrderHandler sender, Order order)
        {
            orderReceived.Enqueue(order);
        }

        internal void Play(Tuple<char, int> columnRow)
        {
            try
            {
                if(!GameState.PossiblesMoves.Contains(columnRow)){
                    return;
                }
                gameHandler.Play(columnRow);
            }
            catch(Exception exception)
            {
                Toolbox.LogError(exception);
            }
        }
    }
}
