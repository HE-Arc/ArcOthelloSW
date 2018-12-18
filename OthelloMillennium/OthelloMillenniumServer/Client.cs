using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace OthelloMillenniumServer
{
    public class Client
    {
        #region Properties
        public TcpClient TcpClient { get; private set; }
        #endregion

        public Client(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        public Client(string hostname, int port)
        {
            if (string.IsNullOrEmpty(hostname))
                throw new ArgumentException("Given hostname is invalid !");

            if (port < 0)
                throw new ArgumentException("Given port is invalid !");

            // Init client
            TcpClient = new TcpClient(hostname, port);
        }
    }
}
