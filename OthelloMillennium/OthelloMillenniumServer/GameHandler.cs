using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OthelloMillenniumServer
{
    class GameHandler
    {
        public ManagedClient client1 { get; private set; }
        public ManagedClient client2 { get; private set; }
        private readonly GameManager gameManager;

        // Events
        public event EventHandler<GameHandlerArgs> OnClientConnectionLost;
        public event EventHandler<GameHandlerArgs> OnClientDisconnected;
        public event EventHandler<GameHandlerArgs> OnClientReconnected;

        public GameHandler(ManagedClient client1, ManagedClient client2)
        {
            this.client1 = client1;
            this.client2 = client2;

            // Init a gameManager
            this.gameManager = new GameManager();
        }
    }

    public class GameHandlerArgs
    {
        public ManagedClient Client { get; set; }
        public DateTime Time { get; private set; } = DateTime.Now;
    }
}
