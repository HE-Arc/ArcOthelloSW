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
        private static object padlock = new object();
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
                    orderHandler.HandleOrder(order);
                }
                Thread.Sleep(50);
            }
        }

        private GameType gameType;
        private GameHandler gameHandler;

        public void JoinGameLocal(PlayerType playerOne, string playerNameOne, PlayerType playerTwo, string playerNameTwo)
        {
            gameHandler = new LocalGameHandler();
            (gameHandler as LocalGameHandler).JoinGame(playerOne, playerNameOne, playerTwo, playerNameTwo);
        }

        public void JoinGameOnline(PlayerType playerOne, string playerNameOne, BattleType battleType)
        {
            gameHandler = new OnlineGameHandler();
            (gameHandler as OnlineGameHandler).JoinGame(playerOne, playerNameOne, battleType);
        }

        public void LaunchGame() => gameHandler.LaunchGame();

        public void HandleOrder(Order order)
        {
            orderReceived.Enqueue(order);
        }
    }
}
