using OthelloMillenniumServer;
using System;
using System.Collections.Generic;
using System.IO;
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
        private Task listenerTask;

        // Informations
        public TcpClient TcpClient { get; private set; }
        public PlayerType Type { get; private set; }
        public Dictionary<string, object> Properties { get; private set; } = new Dictionary<string, object>();

        // Events
        public event EventHandler<OthelloTCPClientArgs> OnOrderReceived;
        public event EventHandler<OthelloTCPClientArgs> OnGameStateReceived;
        public event EventHandler<EventArgs> OnConnectionLost;

        /// <summary>
        /// Basic constructor, start to listen for orders
        /// </summary>
        /// <param name="type">Player or AI</param>
        public OthelloTCPClient(PlayerType type)
        {
            // Store the type of player
            Type = type;

            // Listener task
            listenerTask = new Task(() =>
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
                            if (Receive() is AOrder order && !string.IsNullOrEmpty(order.GetAcronym()))
                                OnOrderReceived?.Invoke(this, new OthelloTCPClientArgs() { Order = order });
                            if (Receive() is GameState gameState)
                                OnGameStateReceived?.Invoke(this, new OthelloTCPClientArgs() { GameState = gameState });
                        }

                        // Wait before reading again
                        Thread.Sleep(100);
                    }
                }
            });

            // Start to listen
            listenerTask.Start();

            // Ping task
            Task pinger = new Task(() =>
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
            });

            // Start to ping
            pinger.Start();
        }

        public void ConnectTo(string serverHostname, int serverPort)
        {
            TcpClient = new TcpClient();

            // Register this client to the server
            TcpClient.Connect(serverHostname, serverPort);
        }

        public void Bind(TcpClient tcpClient)
        {
            this.TcpClient = tcpClient;
        }

        /// <summary>
        /// Send a serialized object to the server
        /// </summary>
        /// <param name="obj">What to transfer</param>
        public void Send(ISerializable obj)
        {
            try
            {
                var stream = TcpClient.GetStream();
                if (stream.CanWrite)
                {
                    BinaryFormatter binaryFmt = new BinaryFormatter();
                    binaryFmt.Serialize(stream, obj);
                }
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);
            }
        }

        /// <summary>
        /// Translate the message waiting on the socket
        /// </summary>
        /// <returns>Deserialized object</returns>
        private object Receive()
        {
            try
            {
                var stream = TcpClient.GetStream();
                var streamReader = new StreamReader(stream);

                if (stream.CanRead)
                {
                    BinaryFormatter binaryFmt = new BinaryFormatter();
                    return binaryFmt.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);
            }

            // Nothing could be read
            return null;
        }
    }

    public class OthelloTCPClientArgs
    {
        public AOrder Order { get; set; }
        public GameState GameState { get; set; }
    }
}
