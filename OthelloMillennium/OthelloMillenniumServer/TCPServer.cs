﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace OthelloMillenniumServer
{
    public class TCPServer
    {
        
        #region Singleton
        private static readonly object padlock = new object();
        private static TCPServer instance = null;

        public static TCPServer Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new TCPServer();
                    }
                }
                return instance;
            }
        }

        private TCPServer() {
            
        }
        #endregion

        #region Properties
        private TcpListener listener; 
        public bool Running { get; private set; }
        public GameType Environnement { get; private set; }
        #endregion

        /// <summary>
        /// Start to listen and accept clients
        /// </summary>
        /// <param name="env">Local or Online</param>
        /// <returns></returns>
        public bool StartListening(GameType env)
        {
            Environnement = env;
            if (Environnement == GameType.Local)
            {
                listener = new TcpListener(IPAddress.Parse(Tools.Properties.Settings.Default.LocalHostname), Tools.Properties.Settings.Default.LocalPort);
            }
            else
            {
                listener = new TcpListener(IPAddress.Parse(Tools.Properties.Settings.Default.OnlineHostname), Tools.Properties.Settings.Default.OnlinePort);
            }

            try
            {
                Running = true;

                // Start to listen
                listener.Start(100);

                // Start an asynchronous socket to listen for connections.  
                Console.WriteLine("Waiting for connections...");

                #region AcceptConnection

                // Accept any new connection
                new Task(() =>
                {
                    // Infinite loop
                    while (Running)
                    {
                        // Accept connection
                        if (listener.Pending())
                        {
                            // Store the new connection inside the client list
                            var newConnection = listener.AcceptTcpClient();

                            // PlayerType will be fetched during the register method inside the matchmaking
                            var client = new OthelloTCPClient();
                            client.Bind(newConnection);

                            // Generate a new player
                            var othelloPlayer = new OthelloPlayerServer(client);
                            othelloPlayer.SetOrderHandler(Matchmaker.Instance);

                            // DEBUG
                            Console.WriteLine("NEW CLIENT CONNECTED");
                        }

                        // Wait before reading again
                        Thread.Sleep(1);
                    }
                }).Start();

                #endregion
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);
            }

            return true;
        }

        /// <summary>
        /// Stop the server and matchmaking process
        /// </summary>
        public void Stop()
        {
            Running = false;
            listener.Stop();
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
}
