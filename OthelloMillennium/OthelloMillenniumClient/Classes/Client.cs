using OthelloMillenniumClient.Classes;
using OthelloMillenniumServer;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumClient
{
    public class Client : OthelloTCPClient
    {
        public event EventHandler<OthelloTCPClientArgs> OnBeginReceived;
        public event EventHandler<OthelloTCPClientArgs> OnAwaitReceived;

        public Client(PlayerType type, string serverHostname, int serverPort)
            : base(type)
        {
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
        public void Search(GameType gameType, PlayerType searchingFor)
        {
            switch(gameType)
            {
                case GameType.Local:
                    this.ConnectTo("localhost", TCPServer.Port);
                    break;
                case GameType.Online:
                    this.ConnectTo(OthelloMillenniumClient.Properties.Settings.Default.OnlineHostname, OthelloMillenniumClient.Properties.Settings.Default.OnlinePort);
                    break;
                default:
                    throw new Exception("Invalid gameType provided");
            }

            switch(searchingFor)
            {
                case PlayerType.AI:
                    var o1 = OrderProvider.SearchBattleAgainstAI as SearchBattleAgainstAIOrder;
                    o1.PlayerType = this.Type;
                    this.Send(o1);
                    break;
                case PlayerType.Human:
                    var o2 = OrderProvider.SearchBattleAgainstPlayer as SearchBattleAgainstPlayerOrder;
                    o2.PlayerType = this.Type;
                    this.Send(o2);
                    break;
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
