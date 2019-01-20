using System;
using Tools;
using Tools.Classes;

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
        public OthelloTCPClient Client1 { get; private set; }

        /// <summary>
        /// Socket server-side linked to a client remote
        /// </summary>
        public OthelloTCPClient Client2 { get; private set; }

        // GameManager
        public GameManager GameManager { get; private set; }

        /// <summary>
        /// Type of game currently being played
        /// </summary>
        public BattleType GameType => GameManager.BattleType;

        public GameHandler(OthelloTCPClient black, OthelloTCPClient white)
        {
            // Init Client 1
            this.Client1 = black;
            this.Client1.Properties.Add("Color", Player.BlackPlayer);
            
            // Init Client 2
            this.Client2 = white;
            this.Client2.Properties.Add("Color", Player.WhitePlayer);

            // Assign opponent to each other
            this.Client1.Properties.Add("Opponent", this.Client2);
            this.Client2.Properties.Add("Opponent", this.Client1);

            // Watch for connection issues
            Client1.OnConnectionLost += Client_OnConnectionLost;
            Client2.OnConnectionLost += Client_OnConnectionLost;

            // Assign color
            Client1.Send(new BlackAssignedOrder());
            Client2.Send(new WhiteAssignedOrder());

            // Is it an AI battle ? Init gameManager
            if ((PlayerType)Client1.Properties["PlayerType"] == PlayerType.Human & (PlayerType)Client2.Properties["PlayerType"] == PlayerType.Human)
                this.GameManager = new GameManager(BattleType.AgainstPlayer);
            else
                this.GameManager = new GameManager(BattleType.AgainstAI);

            // Informs the players that the game is starting
            Client1.Send(new StartOfTheGameOrder());
            Client2.Send(new StartOfTheGameOrder());

            // Start the game
            GameManager.Start();

            // Send the gameboard's initial state
            BroadcastGameboard();

            // Black Start
            ScheduleGameplay();

            // React to clients orders
            Client1.OnOrderReceived += OnOrderReceived;
            Client2.OnOrderReceived += OnOrderReceived;
        }

        private void Client_OnConnectionLost(object sender, EventArgs e)
        {
            var opponent = (sender as OthelloTCPClient).Properties["Opponent"] as OthelloTCPClient;
            opponent.Send(new OpponentConnectionLostOrder());
        }

        private void GameManager_OnGameFinished(object sender, GameState e)
        {
            var gameManager = sender as GameManager;
            var finalGameState = gameManager.Export();

            // Notifiy the end of the game
            Client1.Send(new EndOfTheGameOrder());
            Client2.Send(new EndOfTheGameOrder());

            // Send the final state
            BroadcastGameboard(); 
        }

        // Send begin and await
        private void ScheduleGameplay()
        {
            if ((Player)Client1.Properties["Color"] == GameManager.CurrentPlayerTurn)
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

        // Send current gameState
        private void BroadcastGameboard()
        {
            var gs = GameManager.Export();
            Client1.Send(gs);
            Client2.Send(gs);
        }

        private void OnOrderReceived(object s, OthelloTCPClientArgs e)
        {
            OthelloTCPClient sender = s as OthelloTCPClient;
            OthelloTCPClient opponent = sender == Client1 ? Client2 : Client1;

            if(e.Order is GetCurrentGameStateOrder currentGameStateOrder)
            {
                // A client asked for the gameState, send it back to him
                sender.Send(GameManager.Export());
            }
            else if (e.Order is PlayMoveOrder playMoveOrder)
            {
                // Place a token on the board
                GameManager.PlayMove(playMoveOrder.Coords, (Player)sender.Properties["Color"]);

                // Get the new gameState
                var gs = GameManager.Export();

                // Send current gameState
                BroadcastGameboard();

                // Send being and await
                ScheduleGameplay();

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

                    // Get the new gameState
                    var gs = GameManager.Export();

                    // Send current gameState
                    BroadcastGameboard();

                    // Send being and await
                    ScheduleGameplay();
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

                    // Get the new gameState
                    var gs = GameManager.Export();

                    // Send current gameState
                    BroadcastGameboard();

                    // Send being and await
                    ScheduleGameplay();
                }
                catch
                {
                    sender.Send(new DeniedOrder());
                }
            }
            else
            {
                sender.Send(new DeniedOrder());
            }
        }
    }

    public class GameHandlerArgs
    {
        public OthelloTCPClient Client { get; set; }
        public DateTime Time { get; private set; } = DateTime.Now;
    }
}
