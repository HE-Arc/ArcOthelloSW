using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace OthelloMillenniumServer
{
    public class TCPServer
    {
        
        #region Singleton
        private static readonly object padlock = new object();
        private static TCPServer instance = null;

        public static TCPServer Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new TCPServer();
                    }
                }
                return instance;
            }
        }

        private TCPServer() {
            listener = new TcpListener(IPAddress.Any, Port);
        }
        #endregion

        #region Properties
        private readonly TcpListener listener; 
        public bool Running { get; private set; }
        public int Port { get; set; } = Tools.Properties.Settings.Default.OnlinePort;
        #endregion

        public bool StartListening()
        {
            try
            {
                Running = true;

                // Start to listen
                listener.Start(100);

                // Start an asynchronous socket to listen for connections.  
                Console.WriteLine("Waiting for connections...");

                #region AcceptConnection

                // Accept any new connection
                new Task(async () =>
                {
                    // Infinite loop
                    while (Running)
                    {
                        // Accept connection
                        if (listener.Pending())
                        {
                            // Store the new connection inside the client list
                            var newConnection = listener.AcceptTcpClient();

                            // PlayerType will be fetched during the register method inside the matchmaking
                            var client = new Client(PlayerType.None, "unknown");
                            client.Bind(newConnection);

                            // Will be used once to listen what type of game the player is searching
                            client.OnOrderReceived += Client_OnOrderReceived;

                            // DEBUG
                            Console.WriteLine("NEW CLIENT CONNECTED");
                        }

                        // Wait before reading again
                        await Task.Delay(10);
                    }
                }).Start();

                #endregion
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);

                // Restart Server
                return StartListening();
            }

            return true;
        }

        /// <summary>
        /// Stop the server and matchmaking process
        /// </summary>
        public void Stop()
        {
            Running = false;
            listener.Stop();
        }

        private void Client_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            if (sender is OthelloTCPClient othelloTCPClient)
            {
                if (e.Order is RegisterRequestOrder order)
                {
                    // Register client to the Matchmaker
                    Matchmaker.Instance.RegisterNewClient(othelloTCPClient, order);

                    // TCPServer should only listen once per new connection
                    // It will just listen to know how to annouce the new client to the matchmaker
                    othelloTCPClient.OnOrderReceived -= Client_OnOrderReceived;
                }
            }
        }

        /// <summary>
        /// From : https://stackoverflow.com/questions/409906/can-you-retrieve-the-hostname-and-port-from-a-system-net-sockets-tcpclient
        /// </summary>
        /// <param name="client">An OthelloTCPClient</param>
        /// <returns><see cref="GetHostNameAndPort(TcpClient)"/></returns>
        public Tuple<string, int> GetHostNameAndPort(OthelloTCPClient client)
        {
            return GetHostNameAndPort(client.TcpClient);
        }

        /// <summary>
        /// From : https://stackoverflow.com/questions/409906/can-you-retrieve-the-hostname-and-port-from-a-system-net-sockets-tcpclient
        /// </summary>
        /// <param name="client"></param>
        /// <returns>A Tuple containing Hostname as Item1 and Port as Item2</returns>
        public Tuple<string, int> GetHostNameAndPort(TcpClient client)
        {
            IPEndPoint endPoint = (IPEndPoint)client.Client.LocalEndPoint;
            IPAddress ipAddress = endPoint.Address;

            // get the hostname
            IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
            string hostName = hostEntry.HostName;

            // get the port
            int port = endPoint.Port;

            return new Tuple<string, int>(hostName, port);
        }
    }

  
    public class ServerEvent : EventArgs
    {
        public OthelloTCPClient Client { get; set; }

        public Order Order { get; set; }

        public DateTime FiredDateTime { get; private set; } = DateTime.Now;
    }
}
