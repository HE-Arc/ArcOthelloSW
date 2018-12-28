using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OthelloMillenniumServer
{
    public class Client
    {
        public static TimeSpan TIMEOUT = new TimeSpan(0, 0, 60);

        public enum ManagedState
        {
            Undefined,
            Searching,
            Binded,
            InGame,
            ConnectionLost,
            Disconnected
        }

        #region Properties
        public TcpClient TcpClient { get; private set; }

        // Informations
        public ManagedState State { get; private set; }
        public DateTime RegisterDateTime { get; private set; }
        public DateTime LastValidPingDateTime { get; set; }

        // Events
        public event EventHandler<ClientArgs> OnConnectionLost;
        public event EventHandler<ClientArgs> OnClientBinded;
        public event EventHandler<ClientArgs> OnClientDisconnect;
        #endregion

        private void Init(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            State = ManagedState.Undefined;

            Task t = new Task(() =>
            {
                while (true)
                {
                    if (TcpClient.Connected)
                    {
                        LastValidPingDateTime = DateTime.Now;
                    }
                    else
                    {
                        if(DateTime.Now - LastValidPingDateTime > TIMEOUT)
                        {
                            // TODO
                            State = ManagedState.Disconnected;
                        }
                        else
                        {
                            // TODO
                            State = ManagedState.ConnectionLost;
                            OnConnectionLost?.Invoke(this, new ClientArgs());
                            break;
                        }
                    }

                    // Ping every 5 seconds
                    Thread.Sleep(5000);
                }
            });
        }

        public Client(TcpClient tcpClient)
        {
            Init(tcpClient);
        }

        public Client(string hostname, int port)
        {
            if (string.IsNullOrEmpty(hostname))
                throw new ArgumentException("Given hostname is invalid !");

            if (port < 0)
                throw new ArgumentException("Given port is invalid !");

            // Init client
            Init(new TcpClient(hostname, port));
        }
    }

    public class ClientArgs
    {
        public DateTime Time { get; private set; } = DateTime.Now;
    }
}
