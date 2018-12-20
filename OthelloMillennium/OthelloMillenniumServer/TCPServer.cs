using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OthelloMillenniumServer
{
    public class TCPServer
    {
        public static int Port = 65432;

        #region Singleton
        private static readonly object padlock = new object();
        private static TCPServer instance = null;

        public static TCPServer Instance
        {
            get
            {
                if(instance == null)
                {
                    lock(padlock)
                    {
                        instance = new TCPServer();
                    }
                }
                return instance;
            }
        }

        private TCPServer() { }
        #endregion

        #region Properties
        private readonly TcpListener listener = new TcpListener(IPAddress.Any, Port);
        private readonly List<Client> clients = new List<Client>();
        public event EventHandler<ServerEvent> OnClientConnect;
        public event EventHandler<ServerEvent> OnClientDisconnect;
        #endregion

        public bool StartListening()
        {
            try
            {
                // Start to listen
                listener.Start(100);

                // Start an asynchronous socket to listen for connections.  
                Console.WriteLine("Waiting for connections...");

                Task t = new Task(() =>
                {
                    // Infinite loop
                    while (true)
                    {
                        // Accept connection
                        if (listener.Pending())
                        {
                            // Store the new connection inside the client list
                            var newConnection = new Client(listener.AcceptTcpClient());
                            clients.Add(newConnection);

                            // Fire an event, will be used by the matchmaking
                            OnClientConnect?.Invoke(this, new ServerEvent { Client = newConnection });
                        }

                        // Notify if any client disconnects
                        foreach (Client client in clients)
                        {
                            if (!client.TcpClient.Connected)
                                OnClientDisconnect?.Invoke(this, new ServerEvent { Client = client });
                        }
                    }
                });

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }

            return true;
        }
    }

    public class ServerEvent : EventArgs
    {

        public Client Client { get; set; }

        public DateTime FiredDateTime { get; private set; } = DateTime.Now;
    }
}
