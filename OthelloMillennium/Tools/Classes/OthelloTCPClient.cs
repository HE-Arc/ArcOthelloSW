using OthelloMillenniumServer;
using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Tools.Classes;

namespace Tools
{
    public class OthelloTCPClient
    {
        private BinaryFormatter formatter = new BinaryFormatter();

        // Informations
        public TcpClient TcpClient { get; private set; }

        // Events
        public event EventHandler<OthelloTCPClientArgs> OnOrderReceived;
        public event EventHandler<OthelloTCPClientDataArgs> OnDataReceived;
        public event EventHandler<OthelloTCPClientGameStateArgs> OnGameStateReceived;
        public event EventHandler<OthelloTCPClientSaveArgs> OnSaveReceived;
        public event EventHandler<EventArgs> OnConnectionLost;

        /// <summary>
        /// Basic constructor, start to listen for orders
        /// </summary>
        protected OthelloTCPClient()
        {
            // Listener task
            new Task(() =>
            {
                while (true)
                {
                    if (TcpClient == null)
                    {
                        // Wait 1 second and check TcpConnection again
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        if (TcpClient.Connected)
                        {
                            while (TcpClient.Available > 0)
                            {
                                object streamOutput = Receive();

                                if (streamOutput is Order order && !string.IsNullOrEmpty(order.GetAcronym()))
                                {
                                    OnOrderReceived?.Invoke(this, new OthelloTCPClientArgs(order));
                                }
                                else if (streamOutput is GameState gameState)
                                {
                                    OnGameStateReceived?.Invoke(this, new OthelloTCPClientGameStateArgs(gameState));
                                }
                                else if (streamOutput is Data data)
                                {
                                    OnDataReceived?.Invoke(this, new OthelloTCPClientDataArgs(data));
                                }
                                else if (streamOutput is ExportedGame exportedGame)
                                {
                                    OnSaveReceived?.Invoke(this, new OthelloTCPClientSaveArgs(exportedGame));
                                }
                            }
                        }

                        // Wait before reading again
                        Thread.Sleep(10);
                    }
                }
            }).Start();

            // Ping task
            new Task(() =>
            {
                while (true)
                {
                    if (TcpClient == null)
                    {
                        // Wait 1 second and check TcpConnection again
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        if (!Toolbox.Connected(this))
                        {
                            OnConnectionLost?.Invoke(this, new EventArgs());
                        }
                        Thread.Sleep(5000);
                    }
                }
            }).Start();
        }

        public void ConnectTo(string serverHostname, int serverPort)
        {
            Bind(new TcpClient());

            // Register this client to the server
            TcpClient.Connect(serverHostname, serverPort);
        }

        public void Bind(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        /// <summary>
        /// Send a serialized object to the server
        /// </summary>
        /// <param name="obj">What to transfer</param>
        public void Send(ISerializable obj)
        {
            lock (formatter)
            {
                try
                {
                    formatter.Serialize(TcpClient.GetStream(), obj);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while writing into socket");
                    Toolbox.LogError(ex);
                }
            }
        }

        /// <summary>
        /// Translate the message waiting on the socket
        /// </summary>
        /// <returns>Deserialized object</returns>
        private object Receive()
        {
            lock (formatter)
            {
                try
                {
                    return formatter.Deserialize(TcpClient.GetStream());
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while reading from socket");
                    Toolbox.LogError(ex);
                }

                // Nothing could be read
                return null;
            }
        }
    }

    public class OthelloTCPClientDataArgs
    {
        public Data Data { get; private set; }

        public OthelloTCPClientDataArgs(Data opponentData)
        {
            Data = opponentData;
        }
    }

    public class OthelloTCPClientSaveArgs
    {
        public ExportedGame ExportedGame { get; private set; }

        public OthelloTCPClientSaveArgs(ExportedGame exportedGame)
        {
            ExportedGame = exportedGame;
        }
    }

    public class OthelloTCPClientGameStateArgs
    {
        public GameState GameState { get; set; }

        public OthelloTCPClientGameStateArgs(GameState gameState)
        {
            GameState = gameState;
        }
    }

    public class OthelloTCPClientArgs : EventArgs
    {
        public Order Order { get; private set; }

        public OthelloTCPClientArgs(Order order)
        {
            Order = order;
        }
    }
}
