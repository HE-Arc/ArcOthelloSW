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
        public GameManager.GameType GameType => GameManager.Type;

        public GameHandler(OthelloTCPClient black, OthelloTCPClient white, GameManager.GameType gameType)
        {
            // Init Client
            this.Client1 = black;
            this.Client1.Properties.Add("Color", GameManager.Player.BlackPlayer);
            
            this.Client2 = white;
            this.Client2.Properties.Add("Color", GameManager.Player.WhitePlayer);

            this.Client1.Properties.Add("Opponent", this.Client2);
            this.Client2.Properties.Add("Opponent", this.Client1);

            // Assign color
            Client1.Send(OrderProvider.BlackAssigned);
            Client2.Send(OrderProvider.WhiteAssigned);

            // Init gameManager
            switch(gameType)
            {
                case GameManager.GameType.MultiPlayer:
                    // Init a gameManager
                    this.GameManager = new GameManager(GameManager.GameType.MultiPlayer);

                    break;
                case GameManager.GameType.SinglePlayer:
                    // Init a gameManager
                    this.GameManager = new GameManager(GameManager.GameType.SinglePlayer);

                    //TODO : AI starts or not ?
                    break;
                default:
                    var ex = new Exception("Given gameType is invalid");
                    Toolbox.LogError(ex);
                    throw ex;
            }

            // Informs the players that the game is starting
            Client1.Send(OrderProvider.StartOfTheGame);
            Client2.Send(OrderProvider.StartOfTheGame);

            // Update client state
            Client1.State = PlayerState.InGame;
            Client2.State = PlayerState.InGame;

            // Start the game
            GameManager.Start();

            // Black Start
            Client1.Send(OrderProvider.PlayerBegin);
            Client2.Send(OrderProvider.PlayerAwait);

            // React to clients orders
            Client1.OnOrderReceived += OnOrderReceived;
            Client2.OnOrderReceived += OnOrderReceived;
        }

        private void GameManager_OnGameFinished(object sender, GameState e)
        {
            throw new NotImplementedException();
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
                    GameManager.PlayMove(order.Coords, (GameManager.Player)sender.Properties["Color"]);
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
