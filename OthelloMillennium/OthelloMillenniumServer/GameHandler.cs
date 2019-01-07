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
        public OthelloTCPClient Client1 { get; private set; }
        public OthelloTCPClient Client2 { get; private set; }

        // GameManager
        private readonly GameManager gameManager;
        public GameManager.GameType GameType => gameManager.Type;

        // Events
        public event EventHandler<GameHandlerArgs> OnClientConnectionLost;
        public event EventHandler<GameHandlerArgs> OnClientDisconnected;
        public event EventHandler<GameHandlerArgs> OnClientReconnected;

        public GameHandler(OthelloTCPClient black, OthelloTCPClient white, GameManager.GameType gameType)
        {
            // Init Client
            this.Client1 = black;
            this.Client1.Properties.Add("Color", GameManager.Player.BlackPlayer);

            this.Client2 = white;
            this.Client2.Properties.Add("Color", GameManager.Player.WhitePlayer);

            // Assign color
            TCPServer.Instance.Send(Client1, OrderProvider.BlackAssigned);
            TCPServer.Instance.Send(Client2, OrderProvider.WhiteAssigned);

            // Hook disconnect messages
            TCPServer.Instance.OnClientDisconnect += Instance_OnClientDisconnect;

            // Init gameManager
            switch(gameType)
            {
                case GameManager.GameType.MultiPlayer:
                    // Init a gameManager
                    this.gameManager = new GameManager(GameManager.GameType.MultiPlayer);//TODO: Client send the type of game they would like to play

                    break;
                case GameManager.GameType.SinglePlayer:
                    // Init a gameManager
                    this.gameManager = new GameManager(GameManager.GameType.SinglePlayer);//TODO: Client send the type of game they would like to play

                    //TODO : AI starts or not ?
                    break;
                default:
                    var ex = new Exception("Given gameType is invalid");
                    Toolbox.LogError(ex);
                    throw ex;
            }

            // Informs the players that the game is starting
            TCPServer.Instance.Send(Client1, OrderProvider.StartOfTheGame);
            TCPServer.Instance.Send(Client2, OrderProvider.StartOfTheGame);

            // Start the game
            gameManager.Start();

            // Black Start
            TCPServer.Instance.Send(Client1, OrderProvider.PlayerBegin);
            TCPServer.Instance.Send(Client2, OrderProvider.PlayerAwait);

            // React to clients orders
            Client1.OnOrderReceived += OnOrderReceived;
            Client2.OnOrderReceived += OnOrderReceived;
        }

        private void OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            OthelloTCPClient emitter = sender as OthelloTCPClient;

            switch (e.Order)
            {
                case PlayerBeginOrder nextTurnOrder:
                    break;
                case PlayerAwaitOrder nextTurnOrder:
                    break;
                case NextTurnOrder nextTurnOrder:
                    break;
                //TODO

            }

            throw new NotImplementedException();
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
        public TcpClient Client { get; set; }
        public DateTime Time { get; private set; } = DateTime.Now;
    }
}
