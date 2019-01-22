using System;
using Tools;

namespace OthelloMillenniumServer
{
    /*
     * Parse orders and send them to gamemanager
     * keep clients informed
     */
    public class GameHandler : IOrderHandler
    {
        // Clients
        /// <summary>
        /// Socket server-side linked to a client remote
        /// </summary>
        public OthelloPlayerServer Client1 { get; private set; }

        /// <summary>
        /// Socket server-side linked to a client remote
        /// </summary>
        public OthelloPlayerServer Client2 { get; private set; }

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

        public GameHandler(OthelloPlayerServer black, OthelloPlayerServer white)
        {
            // Init Client 1
            Client1 = black;
            
            // Init Client 2
            Client2 = white;
            
            // TODO SEGAN (NiceToHave): Watch for connection issues

            // Update clients
            Client1.SetColor(Color.Black);
            Client1.SetAvatarID(0);

            Client2.SetColor(Color.White);
            Client2.SetAvatarID(19);

            if (Client1.PlayerType == PlayerType.Human & Client2.PlayerType == PlayerType.Human)
                BattleType = BattleType.AgainstPlayer;
            else
                BattleType = BattleType.AgainstAI;

            // Init gameManager. TODO BASTIEN : Server don't know if it's local or online
            GameManager = new GameManager(GameType.Local);

            // Game is ready
            Client1.GameReady();
            Client2.GameReady();
        }

        private OthelloPlayerServer GetOpponent(OthelloPlayerServer othelloPlayer)
        {
            return othelloPlayer.Equals(Client1) ? Client2 : Client1;
        }

        private void OnConnectionLost(OthelloPlayerServer othelloPlayer)
        {
            if (GetOpponent(othelloPlayer) is OthelloPlayerServer opponent)
            {
                // TODO
                //opponent.Send(new OpponentConnectionLostOrder());
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
            Client1.GameEnded();
            Client2.GameEnded();

            // Send the final state
            BroadcastGameboard(); 
        }

        /// <summary>
        /// Send current gameState
        /// </summary>
        private void BroadcastGameboard()
        {
            var gs = GameManager.Export();
            Client1.UpdateGameboard(gs);
            Client2.UpdateGameboard(gs);
        }

        /// <summary>
        /// Start the game
        /// </summary>
        private void StartGame()
        {
            // Start the game
            GameManager.Start();

            // Informs the players that the game is starting
            Client1.GameStarted();
            Client2.GameStarted();

            // Send gameboard
            BroadcastGameboard();
        }

        private void OnOrderReceived(object s, OthelloTCPClientArgs e)
        {

            if (GetClientAndOpponentFromSender(s, out Client_old sender, out Client_old opponent))
            {

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

        public void SetOrderHandler(IOrderHandler handler)
        {
            throw new Exception("This object can not receive an handler");
        }

        public void HandleOrder(IOrderHandler sender, Order order)
        {
            HandlerOrder(sender as OthelloPlayerServer, order);
        }

        public void HandlerOrder(OthelloPlayerServer sender, Order order)
        {
            switch (order)
            {
                case PlayMoveOrder castedOrder:
                    // Place a token on the board
                    GameManager.PlayMove(castedOrder.Coords, sender.Color);

                    // Send gameBoard to clients
                    BroadcastGameboard();
                    break;

                case Order unknownOrder:
                    throw new Exception("Unknown order received !");
            }
        }
    }
}
