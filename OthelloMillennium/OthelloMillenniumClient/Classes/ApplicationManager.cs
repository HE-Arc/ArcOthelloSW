﻿using OthelloMillenniumClient.Classes.GameHandlers;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        public void GotoMainMenu()
        {
            
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
            try
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
                        if (Lobby == null)
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                //Switch to the windows Game
                                Game game = new Game();
                                game.Show();
                                (Home as Home).Close();
                            });
                        }
                        else
                        {
                            Lobby.OnLaunchGameServer();
                            //Game.OnGameStartServer();
                        }
                        break;

                    case OpponentAvatarChangedOrder order:
                        Lobby.OnUpdateOpponentColorServer(GetPlayers().Color2, order.AvatarID);
                        break;

                    case UpdateGameStateOrder order:
                        GameState = (order as UpdateGameStateOrder).GameState;
                        Game.OnGameStateUpdateServer(GameState);
                        break;

                    case GameEndedOrder order:
                        Game.OnGameEndedServer();
                        break;

                    case SaveResponseOrder order:
                        var dump = GetPlayers();
                        order.SaveFile.Save(dump.Name1, dump.AvatarId1, dump.Color1, dump.Name2, dump.AvatarId2, dump.Color2);
                        break;
                }
            }
            catch (Exception exception)
            {
                Toolbox.LogError(exception);
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

        public void HandleOrder(IOrderHandler sender, Order order)
        {
            orderReceived.Enqueue(order);
        }

        public void Play(Tuple<char, int> columnRow)
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

        public void Undo() => gameHandler.Undo();
        public void Redo() => gameHandler.Redo();

        public void Load()
        {
            gameHandler = new LocalGameHandler();
            gameHandler.SetOrderHandler(this);

            gameHandler.Load();
        }

        /// <summary>
        /// Save a game
        /// </summary>
        public void Save()
        {
            gameHandler.Save();
        }
    }
}
