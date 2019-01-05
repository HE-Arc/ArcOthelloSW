using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OthelloMillenniumServer
{
    class GameHandler
    {
        public TcpClient client1 { get; private set; }
        public TcpClient client2 { get; private set; }
        private readonly GameManager gameManager;

        private bool scheduler = false;

        // Events
        public event EventHandler<GameHandlerArgs> OnClientConnectionLost;
        public event EventHandler<GameHandlerArgs> OnClientDisconnected;
        public event EventHandler<GameHandlerArgs> OnClientReconnected;

        public GameHandler(TcpClient client1, TcpClient client2)
        {
            this.client1 = client1;
            this.client2 = client2;

            // Hook disconnect messages
            TCPServer.Instance.OnClientDisconnect += Instance_OnClientDisconnect;

            // Init a gameManager
            this.gameManager = new GameManager(GameManager.GameType.MultiPlayer);//TODO: Client send the type of game they would like to play
        }

        private void Instance_OnClientDisconnect(object sender, ServerEvent e)
        {
            if(e.Client == client1)
            {
                // client2 win
            }
            else if(e.Client == client2)
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
