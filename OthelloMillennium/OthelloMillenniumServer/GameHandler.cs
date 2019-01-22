using System;
using Tools;

namespace OthelloMillenniumServer
{
    /*
     * Parse orders and send them to gamemanager
     * keep clients informed
     */
    public class GameHandler
    {
        // Clients
        /// <summary>
        /// Socket server-side linked to a client remote
        /// </summary>
        public Client_old Client1 { get; private set; }

        /// <summary>
        /// Socket server-side linked to a client remote
        /// </summary>
        public Client_old Client2 { get; private set; }

        // GameManager
        public GameManager GameManager { get; private set; }

        public BattleType BattleType { get; private set; }

        /// <summary>
        /// Type of game currently being played
        /// </summary>
        public BattleType GameTypeData { get; private set; }

        private readonly object locker = new object();
        private bool client1Ready = false;
        private bool client2Ready = false;

        public GameHandler(Client_old black, Client_old white)
        {
            // Init Client 1
            Client1 = black;
            
            // Init Client 2
            Client2 = white;
            
            // Watch for connection issues
            Client1.OnConnectionLost += Client_OnConnectionLost;
            Client2.OnConnectionLost += Client_OnConnectionLost;

            // React to clients orders
            Client1.OnOrderReceived += OnOrderReceived;
            Client2.OnOrderReceived += OnOrderReceived;

            // Update clients
            Client1.Color = Color.Black;
            Client1.AvatarID = 0;

            Client1.Send(new AssignColorOrder(Client1.Color));
            Client1.Send(new AssignAvatarIDOrder(Client1.AvatarID));

            Client2.Color = Color.White;
            Client2.AvatarID = 19;

            Client2.Send(new AssignColorOrder(Client2.Color));
            Client2.Send(new AssignAvatarIDOrder(Client2.AvatarID));

            if (Client1.PlayerType == PlayerType.Human & Client2.PlayerType == PlayerType.Human)
                BattleType = BattleType.AgainstPlayer;
            else
                BattleType = BattleType.AgainstAI;

            // Init gameManager. TODO BASTIEN : Server don't know if it's local or online
            GameManager = new GameManager(GameType.Local);

            // Game is ready
            Client1.Send(new GameReadyOrder());
            Client2.Send(new GameReadyOrder());
        }

        /// <summary>
        /// Try to get client and opponent from the sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="client"></param>
        /// <param name="opponent"></param>
        /// <returns>True if it succeded</returns>
        private bool GetClientAndOpponentFromSender(object sender, out Client_old client, out Client_old opponent)
        {
            if (sender is Client_old s)
            {
                if (s.Color == Client1.Color)
                {
                    client = Client1;
                    opponent = Client2;
                }
                else
                {
                    client = Client2;
                    opponent = Client1;
                }
                return true;
            }
            else
            {
                client = opponent = null;
                return false;
            }
        }

        private void Client_OnConnectionLost(object sender, EventArgs e)
        {
            if (GetClientAndOpponentFromSender(sender, out Client_old client, out Client_old opponent))
            {
                opponent.Send(new OpponentConnectionLostOrder());
            }
            else
            {
                throw new Exception("Can't cast sender to client");
            }
        }

        private void GameManager_OnGameFinished(object sender, GameState e)
        {
            var gameManager = sender as GameManager;
            var finalGameState = gameManager.Export();

            // Notifiy the end of the game
            Client1.Send(new GameEndedOrder());
            Client2.Send(new GameEndedOrder());

            // Send the final state
            BroadcastGameboard(); 
        }

        /// <summary>
        /// Send begin and await
        /// </summary>
        private void ScheduleGameplay()
        {
            if (Client1.Color == GameManager.CurrentPlayerTurn)
            {
                Client1.Send(new PlayerBeginOrder());
                Client2.Send(new PlayerAwaitOrder());
            }
            else
            {
                Client1.Send(new PlayerAwaitOrder());
                Client2.Send(new PlayerBeginOrder());
            }
        }

        /// <summary>
        /// Send current gameState
        /// </summary>
        private void BroadcastGameboard()
        {
            var gs = GameManager.Export();
            Client1.Send(gs);
            Client2.Send(gs);
        }

        /// <summary>
        /// Start the game
        /// </summary>
        private void StartGame()
        {
            // Start the game
            GameManager.Start();

            // Informs the players that the game is starting
            Client1.Send(new GameStartedOrder());
            Client2.Send(new GameStartedOrder());

            NextTurn();
        }

        /// <summary>
        /// Broadcast gameboard and give hand to opponent
        /// </summary>
        private void NextTurn()
        {
            // Send current gameState
            BroadcastGameboard();

            // Send being and await
            ScheduleGameplay();
        }

        private void OnOrderReceived(object s, OthelloTCPClientArgs e)
        {

            if (GetClientAndOpponentFromSender(s, out Client_old sender, out Client_old opponent))
            {

                if (e.Order is PlayMoveOrder playMoveOrder)
                {
                    // Place a token on the board
                    GameManager.PlayMove(playMoveOrder.Coords, sender.Color);

                    NextTurn();
                }
                else if (e.Order is GetCurrentGameStateOrder currentGameStateOrder)
                {
                    // A client asked for the gameState, send it back to him
                    sender.Send(GameManager.Export());
                }
                else if (e.Order is AvatarChangedOrder avatarChangedOrder)
                {
                    opponent.Send(new OpponentAvatarChangedOrder(avatarChangedOrder.AvatarID));
                }
                else if (e.Order is SaveOrder saveOrder)
                {
                    try
                    {
                        // Get gameStates
                        var output = GameManager.Save();

                        // Send the new gameState to the clients
                        sender.Send(output);
                    }
                    catch
                    {
                        sender.Send(new DeniedOrder());
                    }
                }
                else if (e.Order is UndoOrder undoOrder)
                {
                    try
                    {
                        // Goes one step back
                        GameManager.MoveBack();
                        // TODO SEGAN if with an AI:
                        // two steps back
                        // TODO SEGAN If only AI, not allowed

                        NextTurn();
                    }
                    catch
                    {
                        sender.Send(new DeniedOrder());
                    }
                }
                else if (e.Order is RedoOrder redoOrder)
                {
                    try
                    {
                        // Goes one step back
                        GameManager.MoveForward();

                        NextTurn();
                    }
                    catch
                    {
                        sender.Send(new DeniedOrder());
                    }
                }
                else if (e.Order is PlayerReadyOrder)
                {
                    lock (locker)
                    {
                        if (sender.Color == Color.Black)
                            client1Ready = true;
                        else if (sender.Color == Color.White)
                            client2Ready = true;

                        Console.WriteLine("Player : " + sender.Color);
                        if (client1Ready && client2Ready)
                            StartGame();
                    }
                }
                else
                {
                    sender.Send(new DeniedOrder());
                }
            }
            else
            {
                throw new Exception("Can't cast sender to client");
            }
        }
    }
}
