using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.Classes
{
    class Client
    {
        // Informations
        private TcpClient tcpClient;
        private NetworkStream stream;

        private ConcurrentQueue<Order> orderToSend = new ConcurrentQueue<Order>();
        private ConcurrentQueue<Order> orderReceived = new ConcurrentQueue<Order>();

        private Task taskSender;
        private Task taskListener;
        private Task taskOrderHandler;

        private OrderHandler orderHandler;

        public Client()
        {
            //Nothing
        }

        /// <summary>
        /// Connect to the server and start different services
        /// </summary>
        /// <param name="serverHostname"></param>
        /// <param name="serverPort"></param>
        public void ConnectTo(string serverHostname, int serverPort)
        {
            Bind(new TcpClient());

            // Register this client to the server
            tcpClient.Connect(serverHostname, serverPort);
            stream = tcpClient.GetStream();

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
            if (tcpClient != null)
                throw new ArgumentNullException("tcpClient");
            else
                this.tcpClient = tcpClient;
        }

        /// <summary>
        /// Send order to the server
        /// </summary>
        private void Sender()
        {
            while (!tcpClient.Connected)
            {
                Thread.Sleep(200);
            }

            Console.WriteLine("Server connected, sending enabled");
            while (true)
            {
                while (!orderToSend.IsEmpty)
                {
                    try
                    {
                        orderToSend.TryDequeue(out Order order);

                        // Serialize object
                        byte[] data = null;
                        using (var memoryStream = new MemoryStream())
                        {
                            new BinaryFormatter().Serialize(memoryStream, order);
                            data = memoryStream.ToArray();
                        }

                        // Send Data
                        byte[] userDataLen = BitConverter.GetBytes((Int32)data.Length);
                        stream.Write(userDataLen, 0, 4);
                        stream.Write(data, 0, data.Length);

                        stream.Flush();
                    }
                    catch (Exception exception)
                    {
                        Toolbox.LogError(exception);
                    }
                }
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Listen to the server for messages
        /// </summary>
        private void Listener()
        {
            while (!tcpClient.Connected)
            {
                Thread.Sleep(200);
            }

            Console.WriteLine("Server connected, listening enabled");
            while (true)
            {
                try
                {
                    // Read message length
                    byte[] lengthBuffer = new byte[sizeof(Int32)];
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
                    try
                    {
                        using (var memoryStream = new MemoryStream(data))
                        {
                            order = (Order)new BinaryFormatter().Deserialize(memoryStream);
                        }
                    }
                    catch (SerializationException exception)
                    {
                        //TODO Remove console log juste under this line
                        Console.WriteLine("Error during Serialization ");// + Encoding.Default.GetString(data));
                        Toolbox.LogError(exception);
                    }

                    Console.WriteLine("Received " + order.GetAcronym());

                    orderReceived.Enqueue(order);
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine("Error while reading from socket");
                    Toolbox.LogError(exception);
                }
            }
        }

        /// <summary>
        /// Empty the list of order received
        /// </summary>
        private void OrderHandler()
        {
            while(orderHandler == null)
            {
                Thread.Sleep(50);
            }

            while (true)
            {
                while (!orderReceived.IsEmpty)
                {
                    orderReceived.TryDequeue(out Order order);
                    orderHandler.HandleOrder(order);
                }
                Thread.Sleep(50);
            }
        }
    }
}
