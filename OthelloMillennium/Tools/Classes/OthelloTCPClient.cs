using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace Tools
{
    public class OthelloTCPClient
    {
        private readonly BinaryFormatter formatter = new BinaryFormatter();

        // Informations
        public TcpClient TcpClient { get; private set; }
        
        // Events
        public event EventHandler<OthelloTCPClientArgs> OnOrderReceived;
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
                        // Wait 0.5 second and check TcpConnection again
                        Thread.Sleep(500);
                    }
                    else
                    {
                        var deserializedObject = Receive();

                        if (deserializedObject is Order order && !string.IsNullOrEmpty(order.GetAcronym()))
                        {
                            OnOrderReceived?.Invoke(this, new OthelloTCPClientArgs(order));
                        }
                        else if (deserializedObject is GameState gameState)
                        {
                            OnGameStateReceived?.Invoke(this, new OthelloTCPClientGameStateArgs(gameState));
                        }
                        else if (deserializedObject is ExportedGame exportedGame)
                        {
                            OnSaveReceived?.Invoke(this, new OthelloTCPClientSaveArgs(exportedGame));
                        }
                        else
                        {
                            Console.Error.WriteLine("Can't cast to any known object");
                        }

                        // Avoid flames coming out of cpu
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

        /// <summary>
        /// Attach a tcpClient to this client
        /// </summary>
        /// <param name="tcpClient"></param>
        public void Bind(TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");
            else
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
                if (TcpClient != null && TcpClient.Connected)
                {
                    try
                    {
                        NetworkStream stream = TcpClient.GetStream();

                        Console.WriteLine("Send " + (obj as Order).GetAcronym());

                        // Serialize object
                        formatter.Serialize(stream, obj);

                        // Flush the stream
                        stream.Flush();
                            
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Error while writing into socket");
                        Toolbox.LogError(ex);
                    }
                }
                else
                {
                    Console.Error.WriteLine("TcpClient not connected");
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
                if (TcpClient != null && TcpClient.Connected)
                {
                    try
                    {
                        NetworkStream stream = TcpClient.GetStream();

                        // Deserialize object
                        var deserializedObject = formatter.Deserialize(stream);

                        Console.WriteLine("Received " + (deserializedObject as Order).GetAcronym());

                        // Flush the stream
                        stream.Flush();

                        return deserializedObject;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Error while reading from socket");
                        Toolbox.LogError(ex);
                    }
                }
                else
                {
                    Console.Error.WriteLine("TcpClient not connected");
                }
            }

            return null;
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
