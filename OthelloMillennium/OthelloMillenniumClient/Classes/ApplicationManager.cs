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
                    //orderHandler.HandleOrder(this, order);
                    //TODO
                }
                Thread.Sleep(50);
            }
        }

        public void SetOrderHandler(IOrderHandler handler)
        {
            throw new NotImplementedException();
        }

        private GameHandler gameHandler;

        public void JoinGameLocal(OthelloPlayerClient player1, OthelloPlayerClient player2)
        {
            GameType = GameType.Local;
            gameHandler = new LocalGameHandler();
            (gameHandler as LocalGameHandler).JoinGame(player1, player2);
        }

        public void JoinGameLocal(PlayerType playerTypeOne, string playerNameOne, PlayerType playerTypeTwo, string playerNameTwo)
        {
            GameType = GameType.Local;
            gameHandler = new LocalGameHandler();

            OthelloPlayerClient player1 = new OthelloPlayerClient(playerTypeOne, playerNameOne);
            OthelloPlayerClient player2 = new OthelloPlayerClient(playerTypeTwo, playerNameTwo);

            (gameHandler as LocalGameHandler).JoinGame(player1, player2);
        }

        public void JoinGameOnline(OthelloPlayerClient player, BattleType battleType)
        {
            GameType = GameType.Online;
            gameHandler = new OnlineGameHandler();

            (gameHandler as OnlineGameHandler).JoinGame(player, battleType);
        }

        public void JoinGameOnline(PlayerType playerType, string playerName, BattleType battleType)
        {
            GameType = GameType.Online;
            gameHandler = new OnlineGameHandler();

            OthelloPlayerClient player = new OthelloPlayerClient(playerType, playerName);

            (gameHandler as OnlineGameHandler).JoinGame(player, battleType);
        }

        public void LaunchGame() => gameHandler.LaunchGame();

        public void AvatarIdChange(Color color, int avatarId) => gameHandler.AvatarIdChange(color, avatarId);

        public Tuple<Color, Color> PlayersColor() => gameHandler.PlayersColor();
        public Tuple<int, Color> PlayersAvatarId() => gameHandler.PlayersAvatarId();

        public void HandleOrder(IOrderHandler sender, Order order)
        {
            orderReceived.Enqueue(order);
        }

    }
}
