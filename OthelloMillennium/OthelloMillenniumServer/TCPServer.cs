﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumServer
{
    public class TCPServer
    {
        public static int Port = 65432;

        #region Singleton
        private static readonly object padlock = new object();
        private static TCPServer instance = null;

        public static TCPServer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        instance = new TCPServer();
                    }
                }
                return instance;
            }
        }

        private TCPServer() { }
        #endregion

        #region Properties
        private readonly TcpListener listener = new TcpListener(IPAddress.Any, Port);
        #endregion

        public bool StartListening()
        {
            try
            {
                // Start to listen
                listener.Start(100);

                // Start an asynchronous socket to listen for connections.  
                Console.WriteLine("Waiting for connections...");

                #region AcceptConnection
                // Accept any new connection
                Task t = new Task(() =>
                {
                    // Infinite loop
                    while (true)
                    {
                        // Accept connection
                        if (listener.Pending())
                        {
                            // Store the new connection inside the client list
                            var newConnection = listener.AcceptTcpClient();
                            var client = new OthelloTCPClient();
                            client.Bind(newConnection);

                            // Will be used once to listen what type of game the player is searching
                            client.OnOrderReceived += Client_OnOrderReceived;

                            // DEBUG
                            Console.WriteLine("NEW CLIENT CONNECTED");
                        }
                    }
                });

                // Start to accept new connection
                t.Start();

                #endregion
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);

                // Restart Server
                return StartListening();
            }

            return true;
        }

        private void Client_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            OthelloTCPClient client = sender as OthelloTCPClient;

            // Register client to the Matchmaker
            Matchmaker.Instance.RegisterNewClient(client, e.Order);

            // TCPServer should only listen once per new connection
            // It will just listen to know how to annouce the new client to the matchmaker
            client.OnOrderReceived -= Client_OnOrderReceived; //<-- When activated it react strangely
        }

        /// <summary>
        /// From : https://stackoverflow.com/questions/409906/can-you-retrieve-the-hostname-and-port-from-a-system-net-sockets-tcpclient
        /// </summary>
        /// <param name="client">An OthelloTCPClient</param>
        /// <returns><see cref="GetHostNameAndPort(TcpClient)"/></returns>
        public Tuple<string, int> GetHostNameAndPort(OthelloTCPClient client)
        {
            return GetHostNameAndPort(client.TcpClient);
        }

        /// <summary>
        /// From : https://stackoverflow.com/questions/409906/can-you-retrieve-the-hostname-and-port-from-a-system-net-sockets-tcpclient
        /// </summary>
        /// <param name="client"></param>
        /// <returns>A Tuple containing Hostname as Item1 and Port as Item2</returns>
        public Tuple<string, int> GetHostNameAndPort(TcpClient client)
        {
            IPEndPoint endPoint = (IPEndPoint)client.Client.LocalEndPoint;
            IPAddress ipAddress = endPoint.Address;

            // get the hostname
            IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
            string hostName = hostEntry.HostName;

            // get the port
            int port = endPoint.Port;

            return new Tuple<string, int>(hostName, port);
        }
    }

  
    public class ServerEvent : EventArgs
    {
        public OthelloTCPClient Client { get; set; }

        public AOrder Order { get; set; }

        public DateTime FiredDateTime { get; private set; } = DateTime.Now;
    }
}
