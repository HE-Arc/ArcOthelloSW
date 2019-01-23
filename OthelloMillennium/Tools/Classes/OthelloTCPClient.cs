using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace Tools
{
    public class OthelloTCPClient
    {
        // Informations
        private TcpClient tcpClient;
        private NetworkStream stream;

        private ConcurrentQueue<Order> orderToSend = new ConcurrentQueue<Order>();
        private ConcurrentQueue<Order> orderReceived = new ConcurrentQueue<Order>();

        private Task taskSender;
        private Task taskListener;
        private Task taskOrderHandler;

        private IOrderHandler orderHandler;

        public OthelloTCPClient()
        {
            //Nothing
        }

        public void SetOrderHandler(IOrderHandler orderHandler)
        {
            this.orderHandler = orderHandler;
        }

        /// <summary>
        /// Connect to the server and start different services
        /// </summary>
        /// <param name="serverHostname"></param>
        /// <param name="serverPort"></param>
        public void ConnectTo(string serverHostname, int serverPort)
        {
            if (tcpClient != null)
                throw new Exception("TcpClient already binded");

            tcpClient = new TcpClient();
            
            // Register this client to the server
            tcpClient.Connect(serverHostname, serverPort);

            Start();
        }

        /// <summary>
        /// Start the listener and all services
        /// </summary>
        private void Start()
        {
            if(taskSender != null)
            {
                throw new Exception("Error TcpClient Start called twice");
            }
            taskSender = new Task(Sender);
            taskListener = new Task(Listener);
            taskOrderHandler = new Task(OrderHandler);

            taskSender.Start();
            taskListener.Start();
            taskOrderHandler.Start();
        }

        /// <summary>
        /// Send a serialized object to the server
        /// </summary>
        /// <param name="order">What to transfer</param>
        public void Send(Order order)
        {
            orderToSend.Enqueue(order);
        }

        /// <summary>
        /// Attach a tcpClient to this client
        /// </summary>
        /// <param name="tcpClient"></param>
        public void Bind(TcpClient tcpClient)
        {
            if (this.tcpClient != null)
                throw new Exception("TcpClient already binded");

            this.tcpClient = tcpClient ?? throw new ArgumentNullException("tcpClient");

            Start();
        }

        /// <summary>
        /// Send order to the server
        /// </summary>
        private void Sender()
        {
            Console.WriteLine("Starting sender task");
            while (true)
            {
                if (IsConnected())
                { 
                    // Get the stream
                    stream = tcpClient.GetStream();

                    while (!orderToSend.IsEmpty)
                    {
                        try
                        {
                            orderToSend.TryDequeue(out Order order);
                            Console.WriteLine("Send order " + order.GetAcronym());
                            // Serialize object
                            byte[] data = null;
                            using (var memoryStream = new MemoryStream())
                            {
                                new BinaryFormatter().Serialize(memoryStream, order);
                                data = memoryStream.ToArray();
                            }

                            // Send Data
                            byte[] userDataLen = BitConverter.GetBytes(data.Length);
                            stream.Write(userDataLen, 0, 4);
                            stream.Write(data, 0, data.Length);

                            stream.Flush();
                        }
                        catch (Exception exception)
                        {
                            Toolbox.LogError(exception);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(450);
                }
            }
        }

        public bool IsConnected()
        {
            return tcpClient != null && tcpClient.Connected;
        }

        /// <summary>
        /// Listen to the server for messages
        /// </summary>
        private void Listener()
        {
            Console.WriteLine("Starting listener task");
            while (true)
            {
                if (IsConnected() && tcpClient.Available > 0)
                {
                    // Get the stream
                    stream = tcpClient.GetStream();
                    try
                    {
                        // Read message length
                        byte[] lengthBuffer = new byte[sizeof(int)];
                        int recv = stream.Read(lengthBuffer, 0, lengthBuffer.Length);

                        // Prepare receiving
                        byte[] data = null;

                        if (recv == sizeof(int))
                        {
                            int messageLen = BitConverter.ToInt32(lengthBuffer, 0);
                            data = new byte[messageLen];
                            recv = stream.Read(data, 0, data.Length);

                            if (recv != messageLen)
                            {
                                //Adapt size workaround
                                Console.WriteLine("Shit missing some data" + messageLen);
                            }
                        }

                        Order order = null;
                        using (var memoryStream = new MemoryStream(data))
                        {
                            order = (Order)new BinaryFormatter().Deserialize(memoryStream);
                        }

                        Console.WriteLine("Received " + order.GetAcronym());
                        orderReceived.Enqueue(order);
                    }
                    catch (SerializationException exception)
                    {
                        //TODO Remove console log juste under this line
                        Console.WriteLine("Error during Serialization ");// + Encoding.Default.GetString(data));
                        Toolbox.LogError(exception);
                    }
                    catch (Exception exception)
                    {
                        Console.Error.WriteLine("Error while reading from socket");
                        Toolbox.LogError(exception);
                    }
                }
                else
                {
                    Thread.Sleep(450);
                }
            }
        }

        /// <summary>
        /// Empty the list of order received
        /// </summary>
        private void OrderHandler()
        {
            while (true)
            {
                if(orderHandler != null)
                { 
                    while (!orderReceived.IsEmpty)
                    {
                        orderReceived.TryDequeue(out Order order);
                        orderHandler.HandleOrder(null, order);
                    }
                }
                
                Thread.Sleep(50);
            }
        }
    }
}
