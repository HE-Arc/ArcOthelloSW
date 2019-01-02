using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OthelloMillenniumServer
{
    public class Matchmaker
    {
        #region Internal Classes
        

        #endregion

        #region Singleton
        private static readonly object padlock = new object();
        private static Matchmaker instance = null;

        public static Matchmaker Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        instance = new Matchmaker();
                    }
                }
                return instance;
            }
        }

        private Matchmaker() {
            // Every time the server will accept a new connection we will register him as a new client
            TCPServer.Instance.OnClientConnect += RegisterNewClient;
            TCPServer.Instance.OnClientDisconnect += DisconnectClient;
        }
        #endregion

        #region Attributes
        private readonly HashSet<GameHandler> matches = new HashSet<GameHandler>();
        private readonly HashSet<TcpClient> clients = new HashSet<TcpClient>();
        private bool running = false;
        #endregion

        #region Properties
        
        #endregion

        private void StartMatchmaking()
        {
            if(!running)
            {
                running = true;
                Task binderThread = new Task(() =>
                {
                    while(running)
                    {
                        // Start to look for any good binding if there is 2 or more player/AI waiting
                        if (clients.Count > 2)
                        {
                            // TODO : Insert logic (i.e. ranking, waiting time, etc)

                            // For now we take the first and the last one
                            var client1 = clients.First();
                            var client2 = clients.Last();

                            // GameManager will now handle clients and put them as InGame
                            matches.Add(new GameHandler(client1, client2));

                            // Informs clients that an opponent has be found
                            TCPServer.Instance.Send(client1, Toolbox.Protocole.OpponentFound);
                            TCPServer.Instance.Send(client2, Toolbox.Protocole.OpponentFound);

                            // Remove them from the queue
                            clients.Remove(client1);
                            clients.Remove(client2);
                        }

                        // Sleep for 500 ms
                        Thread.Sleep(500);
                    }
                });
            }
            else
            {
                throw new Exception("Matchmaking already started !");
            }
        }

        private void RegisterNewClient(object sender, ServerEvent e)
        {
            if (clients.Contains(e.Client))
            {
                var currentMatch = matches.Where(match => match.client1 == e.Client || match.client2 == e.Client).First();
                if (currentMatch == null)
                {
                    throw new Exception("Client already registred and searching for a game");
                }
                else
                {
                    // Reconnect to the game
                    // ---------------------
                    // Check timers
                    // Disconnected for too long ?
                }
            }
            else
            {
                // Register new client
                clients.Add(e.Client);

                // Informs the client that he is now known to the server
                TCPServer.Instance.Send(e.Client, Toolbox.Protocole.RegisterSuccessful);
            }
        }

        private void DisconnectClient(object sender, ServerEvent e)
        {
            if (clients.Contains(e.Client))
            {
                var currentMatch = matches.Where(match => match.client1 == e.Client || match.client2 == e.Client).First();

                if (currentMatch == null)
                {
                    clients.Remove(e.Client);
                }
                else
                {
                    // Disconnect handled in gameHandler
                    // ---------------------
                }
            }
            else
            {
                Toolbox.LogError(new Exception("Client was not registred"));
            }
        }
    }
}
