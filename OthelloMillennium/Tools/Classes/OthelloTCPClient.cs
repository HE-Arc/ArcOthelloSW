using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace Tools
{
    public class OthelloTCPClient
    {
        #region Attributes
        private readonly BinaryFormatter formatter = new BinaryFormatter();
        private readonly ConcurrentQueue<ISerializable> requests = new ConcurrentQueue<ISerializable>();
        #endregion

        // Informations
        private TcpClient tcpClient;
        
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
                    if (!IsConnected())
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
                    if (tcpClient == null)
                    {
                        // Wait 1 second and check TcpConnection again
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        if (!tcpClient.Connected)
                        {
                            OnConnectionLost?.Invoke(this, new EventArgs());
                        }

                        Thread.Sleep(5000);
                    }
                }
            }).Start();

            // Sender task
            new Task(() =>
            {
                while (true)
                {
                    if (!IsConnected())
                    {
                        // Wait 0.5 second and check TcpConnection again
                        Thread.Sleep(500);
                    }
                    else
                    {
                        // Order over object
                        if(requests.Count > 0)
                        {
                            requests.TryDequeue(out ISerializable output);
                            Write(output);
                        }

                        // Avoid flames coming out of cpu
                        Thread.Sleep(10);
                    }
                }
            }).Start();
        }

        public void ConnectTo(string serverHostname, int serverPort)
        {
            Bind(new TcpClient());

            // Register this client to the server
            tcpClient.Connect(serverHostname, serverPort);
        }

        /// <summary>
        /// Attach a tcpClient to this client
        /// </summary>
        /// <param name="othelloTCPClient"></param>
        public void Bind(OthelloTCPClient othelloTCPClient)
        {
            Bind(othelloTCPClient.tcpClient);
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
                this.tcpClient = tcpClient;
        }

        /// <summary>
        /// Send a serialized object to the server
        /// </summary>
        /// <param name="obj">What to transfer</param>
        public void Send(ISerializable serializable)
        {
            requests.Enqueue(serializable);
        }

        private void Write(ISerializable serializable)
        {
            lock (formatter)
            {
                try
                {
                    Console.WriteLine("Send " + (serializable as Order).GetAcronym());

                    // Get the stream
                    NetworkStream stream = tcpClient.GetStream();

                    // Serialize object
                    formatter.Serialize(stream, serializable);

                    // Flush the stream
                    stream.Flush();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while writing into socket");
                    Toolbox.LogError(ex);
                }
            }
        }

        public bool IsConnected()
        {
            return tcpClient != null && tcpClient.Connected;
        }

        /// <summary>
        /// Translate the message waiting on the socket
        /// </summary>
        /// <returns>Deserialized object</returns>
        private object Receive()
        {
            object deserializedObject = null;

            lock (formatter)
            {
                try
                {
                    NetworkStream stream = tcpClient.GetStream();

                    // Deserialize object
                    deserializedObject = formatter.Deserialize(stream);

                    Console.WriteLine("Received " + (deserializedObject as Order).GetAcronym());

                    // Flush the stream
                    stream.Flush();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while reading from socket");
                    Toolbox.LogError(ex);
                }
            }

            return deserializedObject;
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
