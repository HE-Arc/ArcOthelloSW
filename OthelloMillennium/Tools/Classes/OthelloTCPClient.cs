using OthelloMillenniumServer;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tools.Classes;

namespace Tools
{
    public class OthelloTCPClient
    {  
        // Informations
        public TcpClient TcpClient { get; private set; }
        public PlayerState State { get; private set; }

        public Dictionary<string, object> Properties { get; private set; } = new Dictionary<string, object>();

        // Events
        public event EventHandler<OthelloTCPClientArgs> OnOrderReceived;

        public OthelloTCPClient()
        {

        }

        public void Connect(string serverHostname, int serverPort)
        {
            TcpClient = new TcpClient();
            State = PlayerState.Undefined;

            // Register this client to the server
            TcpClient.Connect(serverHostname, serverPort);
        }

        public void Bind(TcpClient tcpClient)
        {
            this.TcpClient = tcpClient;
        }

        /// <summary>
        /// Please use Toolbox.Protocole in order to choose the correct message to wait for
        /// </summary>
        /// <param name="message"></param>
        public void WaitForOrder(IOrder order)
        {
            // Wait for the confirmation to be registred
            Task waitForOrder = new Task(() =>
            {
                var stream = TcpClient.GetStream();
                while (true)
                {
                    if (stream.CanRead)
                    {
                        byte[] buffer = new byte[order.GetLength()];

                        // Read the stream
                        string output = this.Receive(order);

                        if (!string.IsNullOrEmpty(output))
                        {
                            if (output == order.GetAcronym())
                            {
                                OnOrderReceived?.Invoke(this, new OthelloTCPClientArgs() { Order = order });
                            }
                            else
                            {
                                Toolbox.LogError(new Exception($"Wrong message received!\r\nWaiting for {order.GetAcronym()}"));
                            }
                        }
                        Thread.Sleep(100);
                    }
                }
            });
        }

        /// <summary>
        /// Send a message to the server
        /// </summary>
        /// <param name="message">What to transfer</param>
        public void Send(IOrder order)
        {
            try
            {
                var stream = TcpClient.GetStream();
                if (stream.CanWrite)
                {
                    byte[] vs = Encoding.ASCII.GetBytes(order.GetAcronym());
                    try
                    {
                        stream.Write(vs, 0, order.GetLength());
                    }
                    catch (Exception ex)
                    {
                        Toolbox.LogError(ex);
                    }
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
        /// <returns>string : the message received</returns>
        private string Receive(IOrder order)
        {
            try
            {
                var stream = TcpClient.GetStream();
                if (stream.CanRead)
                {
                    byte[] buffer = new byte[order.GetLength()];
                    stream.Read(buffer, 0, buffer.Length);

                    // Return the decode message
                    return Encoding.UTF8.GetString(buffer);
                }
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);
            }

            // Nothing could be read
            return string.Empty;
        }
    }

    public class OthelloTCPClientArgs
    {
        public IOrder Order { get; set; }
    }
}
