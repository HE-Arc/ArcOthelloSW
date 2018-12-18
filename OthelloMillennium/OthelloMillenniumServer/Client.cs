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

        private Client(ref TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        private Client(string hostname, int port)
        {
            if (string.IsNullOrEmpty(hostname))
                throw new ArgumentException("Given hostname is invalid !");

            if (port < 0)
                throw new ArgumentException("Given port is invalid !");

            // Init client
            TcpClient = new TcpClient(hostname, port);
        }

        public static Client FromTCPClient(ref TcpClient tcpClient)
        {
            return new Client(ref tcpClient);
        }

        public static Client FromHostname(string hostname, int port)
        {
            return new Client(hostname, port);
        }
    }
}
