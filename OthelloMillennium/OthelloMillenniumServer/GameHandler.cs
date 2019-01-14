using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            Client1.Send(OrderProvider.BlackAssigned);
            Client2.Send(OrderProvider.WhiteAssigned);

            // Is it an AI battle ? Init gameManager
            if (Client1.Type == PlayerType.Human & Client2.Type == PlayerType.Human)
                this.GameManager = new GameManager(BattleType.AgainstPlayer);
            else
                this.GameManager = new GameManager(BattleType.AgainstAI);

            // Informs the players that the game is starting
            Client1.Send(OrderProvider.StartOfTheGame);
            Client2.Send(OrderProvider.StartOfTheGame);

            // Start the game
            GameManager.Start();

            // Black Start
            Client1.Send(OrderProvider.PlayerBegin);
            Client2.Send(OrderProvider.PlayerAwait);

            // Send current gameState
            var gs = GameManager.Export();
            Client1.Send(gs);
            Client2.Send(gs);

            // React to clients orders
            Client1.OnOrderReceived += OnOrderReceived;
            Client2.OnOrderReceived += OnOrderReceived;
        }

        private void Client_OnConnectionLost(object sender, EventArgs e)
        {
            var opponent = (sender as OthelloTCPClient).Properties["Opponent"] as OthelloTCPClient;
            opponent.Send(OrderProvider.OpponentConnectionLost);
        }

        private void GameManager_OnGameFinished(object sender, GameState e)
        {
            var gameManager = sender as GameManager;
            var finalGameState = gameManager.Export();

            // Notifiy the end of the game
            Client1.Send(OrderProvider.EndOfTheGame);
            Client2.Send(OrderProvider.EndOfTheGame);

            // Send the final state
            Client1.Send(finalGameState);
            Client2.Send(finalGameState);   
        }

        private void OnOrderReceived(object s, OthelloTCPClientArgs e)
        {
            OthelloTCPClient sender = s as OthelloTCPClient;
            OthelloTCPClient opponent = sender == Client1 ? Client2 : Client1;

            switch (e.Order)
            {
                case GetCurrentGameStateOrder order:
                    // A client asked for the gameState, send it back to him
                    sender.Send(GameManager.Export());
                    break;

                case NextTurnOrder order:
                    // Inform opponent that he can start to play.
                    sender.Send(OrderProvider.PlayerAwait);
                    opponent.Send(OrderProvider.PlayerBegin);
                    break;

                case PlayMoveOrder order:
                    GameManager.PlayMove(order.Coords, (Player)sender.Properties["Color"]);
                    var gs = GameManager.Export();

                    // Send current gameState
                    sender.Send(gs);
                    opponent.Send(gs);
                    break;

                default:
                    throw new Exception("Unknown order");
                //TODO

            }
        }
    }

    public class GameHandlerArgs
    {
        public OthelloTCPClient Client { get; set; }
        public DateTime Time { get; private set; } = DateTime.Now;
    }
}
