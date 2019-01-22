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

        private IOrderHandler orderHandler;

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

        private GameType gameType;

        private ApplicationManager()
        {
            orderReceived = new ConcurrentQueue<Order>();

            taskOrderHandler = new Task(ManageOrder);
            taskOrderHandler.Start();
        }

        private void ManageOrder()
        {
            while (orderHandler == null)
            {
                Thread.Sleep(50);
            }

            while (true)
            {
                while (!orderReceived.IsEmpty)
                {
                    orderReceived.TryDequeue(out Order order);
                    orderHandler.HandleOrder(this, order);
                }
                Thread.Sleep(50);
            }
        }

        public void SetOrderHandler(IOrderHandler handler)
        {
            throw new NotImplementedException();
        }

        private GameHandler gameHandler;

        public void JoinGameLocal(PlayerType playerOne, string playerNameOne, PlayerType playerTwo, string playerNameTwo)
        {
            gameType = GameType.Local;
            gameHandler = new LocalGameHandler();
            (gameHandler as LocalGameHandler).JoinGame(playerOne, playerNameOne, playerTwo, playerNameTwo);
        }

        public void JoinGameOnline(PlayerType playerOne, string playerNameOne, BattleType battleType)
        {
            gameType = GameType.Online;
            gameHandler = new OnlineGameHandler();
            (gameHandler as OnlineGameHandler).JoinGame(playerOne, playerNameOne, battleType);
        }

        public void LaunchGame() => gameHandler.LaunchGame();

        public void HandleOrder(IOrderHandler sender, Order order)
        {
            orderReceived.Enqueue(order);
        }

    }
}
