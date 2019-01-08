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
        private readonly GameManager gameManager;

        /// <summary>
        /// Type of game currently being played
        /// </summary>
        public GameManager.GameType GameType => gameManager.Type;

        public GameHandler(OthelloTCPClient black, OthelloTCPClient white, GameManager.GameType gameType)
        {
            // Init Client
            this.Client1 = black;
            this.Client1.Properties.Add("Color", GameManager.Player.BlackPlayer);

            this.Client2 = white;
            this.Client2.Properties.Add("Color", GameManager.Player.WhitePlayer);

            // Assign color
            Client1.Send(OrderProvider.BlackAssigned);
            Client2.Send(OrderProvider.WhiteAssigned);

            // Hook disconnect messages
            TCPServer.Instance.OnClientDisconnect += Instance_OnClientDisconnect;

            // Init gameManager
            switch(gameType)
            {
                case GameManager.GameType.MultiPlayer:
                    // Init a gameManager
                    this.gameManager = new GameManager(GameManager.GameType.MultiPlayer);

                    break;
                case GameManager.GameType.SinglePlayer:
                    // Init a gameManager
                    this.gameManager = new GameManager(GameManager.GameType.SinglePlayer);

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

            // Start the game
            gameManager.Start();

            // Black Start
            Client1.Send(OrderProvider.PlayerBegin);
            Client2.Send(OrderProvider.PlayerAwait);

            // React to clients orders
            Client1.OnOrderReceived += OnOrderReceived;
            Client2.OnOrderReceived += OnOrderReceived;
        }

        private void OnOrderReceived(object s, OthelloTCPClientArgs e)
        {
            OthelloTCPClient sender = s as OthelloTCPClient;
            OthelloTCPClient opponent = sender == Client1 ? Client2 : Client1;

            switch (e.Order)
            {
                case GetCurrentGameStateOrder order:
                    // A client asked for the gameState, send it back to him
                    sender.Send(gameManager.Export());
                    break;

                case NextTurnOrder order:
                    // Inform opponent that he can start to play.
                    sender.Send(OrderProvider.PlayerAwait);
                    opponent.Send(OrderProvider.PlayerBegin);
                    break;

                case PlayMoveOrder order:
                    gameManager.PlayMove(order.Coords, (GameManager.Player)sender.Properties["Color"]);
                    var gs = gameManager.Export();

                    // Send current gameState
                    sender.Send(gs);
                    opponent.Send(gs);
                    break;

                default:
                    throw new Exception("Unknown order");
                //TODO

            }
        }

        private void Instance_OnClientDisconnect(object sender, ServerEvent e)
        {
            if(e.Client == Client1)
            {
                // client2 win
            }
            else if(e.Client == Client2)
            {
                // client1 win
            }
            else
            {
                // Do Nothing
            }
        }
    }

    public class GameHandlerArgs
    {
        public OthelloTCPClient Client { get; set; }
        public DateTime Time { get; private set; } = DateTime.Now;
    }
}
