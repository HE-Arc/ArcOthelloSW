using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumServer
{
    public class Client : OthelloTCPClient
    {
        public event EventHandler<OthelloTCPClientArgs> OnBeginReceived;
        public event EventHandler<OthelloTCPClientArgs> OnAwaitReceived;

        public Client(string serverHostname, int serverPort)
            : base()
        {
            this.ConnectTo(serverHostname, serverPort);
            this.OnOrderReceived += Client_OnOrderReceived;
        }

        private void Client_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            switch(e.Order)
            {
                case PlayerAwaitOrder order:
                    OnAwaitReceived?.Invoke(this, e);
                    break;
                case PlayerBeginOrder order:
                    OnBeginReceived?.Invoke(this, e);
                    break;
            }
        }

        /// <summary>
        /// Send a message to the binded server in order to register itself
        /// </summary>
        /// <param name="gameType">game type you're looking for</param>
        public void Search(GameManager.GameType gameType)
        {
            switch(gameType)
            {
                case GameManager.GameType.SinglePlayer:
                    this.Send(OrderProvider.SearchLocalGame);
                    break;
                case GameManager.GameType.MultiPlayer:
                    this.Send(OrderProvider.SearchOnlineGame);
                    break;
                default:
                    throw new Exception("Invalid gameType provided");
            }
        }

        /// <summary>
        /// Sends to the server the location where the new token has been placed
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="column">column</param>
        public void Play(char row, int column)
        {
            var order = OrderProvider.PlayMove as PlayMoveOrder;

            // Insert data into order
            order.Coords = new Tuple<char, int>(row, column);

            this.Send(order);
        }
    }
}
