using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools
{
    public class OthelloTCPClient
    {
        private readonly BinaryFormatter formatter = new BinaryFormatter();
        private MemoryStream memoryStreamListen = new MemoryStream();

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
            StartListener();

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

                        // Serialize object
                        Message message = Serializer.Serialize(obj);

                        //Workaround because of periodic random lost of bytes during communication transfert
                        byte[] array1 = new byte[4];
                        array1[0] = 8;
                        array1[1] = 8;
                        array1[2] = 8;
                        array1[3] = 8;
                        byte[] array2 = message.Data;
                        byte[] newArray = new byte[array1.Length + array2.Length];
                        Array.Copy(array1, newArray, array1.Length);
                        Array.Copy(array2, 0, newArray, array1.Length, array2.Length);

                        // Send Data
                        byte[] userDataLen = BitConverter.GetBytes((Int32)newArray.Length);
                        stream.Write(userDataLen, 0, 4);
                        stream.Write(newArray, 0, newArray.Length);

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

        private void StartListener()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

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
                        Listener();
                    }
                }
            }).Start();
        }

        private void ManageOrder(object obj)
        {
            if (obj is Order order && !string.IsNullOrEmpty(order.GetAcronym()))
            {
                OnOrderReceived?.Invoke(this, new OthelloTCPClientArgs(order));
            }
            else if (obj is GameState gameState)
            {
                OnGameStateReceived?.Invoke(this, new OthelloTCPClientGameStateArgs(gameState));
            }
            else if (obj is ExportedGame exportedGame)
            {
                OnSaveReceived?.Invoke(this, new OthelloTCPClientSaveArgs(exportedGame));
            }
            else
            {
                Console.Error.WriteLine("Can't cast to any known object");
            }
        }

        /// <summary>
        /// Translate the message waiting on the socket
        /// </summary>
        /// <returns>Deserialized object</returns>
        private object Listener()
        {
            NetworkStream stream = TcpClient.GetStream();
            while (true)
            {
                while (!TcpClient.GetStream().DataAvailable)
                {
                    Thread.Sleep(10);
                }
                if (TcpClient != null && TcpClient.Connected)
                {
                    try
                    {
                        Message message = new Message();

                        byte[] lengthBuffer = new byte[sizeof(Int32)];
                        int recv = stream.Read(lengthBuffer, 0, lengthBuffer.Length);
                        if (recv == sizeof(int))
                        {
                            int messageLen = BitConverter.ToInt32(lengthBuffer, 0);
                            byte[] messageBuffer = new byte[messageLen];
                            recv = stream.Read(messageBuffer, 0, messageBuffer.Length);
                            if (recv == messageLen)
                            {
                                //Remove 4 bytes Workaround
                                message.Data = messageBuffer.Skip(4).ToArray();
                            }
                            else
                            {
                                //Adapt size workaround
                                message.Data = messageBuffer.Take(messageLen-4).ToArray();
                                Console.WriteLine("Missing part of data workaround"+messageLen);
                            }
                        }
                        
                        object deserializedObject = null;
                        try
                        {
                            deserializedObject = Serializer.Deserialize(message);
                        }
                        catch (SerializationException exception)
                        {
                            Console.WriteLine("Error during Serialization " + Encoding.Default.GetString(message.Data));
                            Toolbox.LogError(exception);
                        }

                        Console.WriteLine("Received " + (deserializedObject as Order).GetAcronym());

                        // Flush the stream
                        stream.Flush();

                        ManageOrder(deserializedObject);
                    }
                    catch (Exception exception)
                    {
                        Console.Error.WriteLine("Error while reading from socket");
                        Toolbox.LogError(exception);
                    }
                }
                else
                {
                    Console.Error.WriteLine("TcpClient not connected");
                }
            }

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
