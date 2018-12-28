using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
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
        private readonly List<TcpClient> clients = new List<TcpClient>();
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

                // Accept any new connection
                Task t = new Task(() =>
                {
                    // Infinite loop
                    while (true)
                    {
                        // Accept connection
                        if (listener.Pending())
                        {
                            // Store the new connection inside the client list
                            var newConnection = listener.AcceptTcpClient();
                            clients.Add(newConnection);

                            // Fire an event, will be used by the matchmaking
                            OnClientConnect?.Invoke(this, new ServerEvent { Client = newConnection });
                        }
                    }
                });

                // Every 5 seconds the server will ping clients
                Task pinger = new Task(() =>
                {
                    // Notify if any client disconnects
                    foreach (TcpClient client in clients)
                    {
                        if (!Ping(client))
                            OnClientDisconnect?.Invoke(this, new ServerEvent { Client = client });
                    }

                    // Ping every 5 seconds
                    Thread.Sleep(5000);
                });

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }

            return true;
        }

        public bool Send(TcpClient client, string message)
        {
            if(client.Connected)
            {
                var stream = client.GetStream();
                if(stream.CanWrite)
                {
                    byte[] vs = Encoding.ASCII.GetBytes(message);
                    try
                    {
                        stream.Write(vs, 0, vs.Length);
                    }
                    catch (Exception ex)
                    {
                        Toolbox.LogError(ex);
                    }

                    // Message send
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// From : https://stackoverflow.com/questions/409906/can-you-retrieve-the-hostname-and-port-from-a-system-net-sockets-tcpclient
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
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

        public bool Ping(TcpClient client)
        {
            Ping p = new Ping();
            var hp = GetHostNameAndPort(client);
            var pr = p.Send(hp.Item1, Settings.TIMEOUT);
            return pr.Status == IPStatus.Success;
        }
    }

  
    public class ServerEvent : EventArgs
    {

        public TcpClient Client { get; set; }

        public DateTime FiredDateTime { get; private set; } = DateTime.Now;
    }
}
