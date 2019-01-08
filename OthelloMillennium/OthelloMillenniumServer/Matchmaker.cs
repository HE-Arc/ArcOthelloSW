using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumServer
{
    public class Matchmaker
    {
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
        private readonly Dictionary<GameManager.GameType, HashSet<OthelloTCPClient>> registratedClientsPerGameTypeDict = new Dictionary<GameManager.GameType, HashSet<OthelloTCPClient>>();
        private bool running = false;
        #endregion

        #region Properties
        HashSet<OthelloTCPClient> LocalClients => registratedClientsPerGameTypeDict[GameManager.GameType.SinglePlayer];
        HashSet<OthelloTCPClient> OnlineClients => registratedClientsPerGameTypeDict[GameManager.GameType.MultiPlayer];
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
                        #region Local
                        // Start to look for any good binding if there is 2 or more player/AI waiting
                        if (LocalClients.Count > 2)
                        {
                            // TODO : Insert logic (i.e. ranking, waiting time, etc)

                            // For now we take the first and the last one
                            var client1 = LocalClients.First();
                            var client2 = LocalClients.Last();

                            // GameManager will now handle clients and put them as InGame
                            matches.Add(new GameHandler(client1, client2, GameManager.GameType.SinglePlayer));

                            // Informs clients that an opponent has be found
                            client1.Send(OrderProvider.OpponentFound);
                            client2.Send(OrderProvider.OpponentFound);

                            // Remove them from the queue
                            LocalClients.Remove(client1);
                            LocalClients.Remove(client2);
                        }
                        #endregion

                        #region Online
                        // Start to look for any good binding if there is 2 or more player/AI waiting
                        if (OnlineClients.Count > 2)
                        {
                            // TODO : Insert logic (i.e. ranking, waiting time, etc)

                            // For now we take the first and the last one
                            var client1 = OnlineClients.First();
                            var client2 = OnlineClients.Last();

                            // GameManager will now handle clients and put them as InGame
                            matches.Add(new GameHandler(client1, client2, GameManager.GameType.MultiPlayer));

                            // Informs clients that an opponent has be found
                            client1.Send(OrderProvider.OpponentFound);
                            client2.Send(OrderProvider.OpponentFound);

                            // Remove them from the queue
                            OnlineClients.Remove(client1);
                            OnlineClients.Remove(client2);
                        }
                        #endregion

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

        /// <summary>
        /// Retrieves the match in which the player plays
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public GameHandler GetMatch(OthelloTCPClient client)
        {
            return matches.Where(match => match.Client1 == client || match.Client2 == client).FirstOrDefault();
        }

        public bool IsKnown(OthelloTCPClient client)
        {
            return LocalClients.Contains(client) || OnlineClients.Contains(client);
        }

        private void RegisterNewClient(object sender, ServerEvent e)
        {
            if (IsKnown(e.Client))
            {
                var currentMatch = matches.Where(match => match.Client1 == e.Client || match.Client2 == e.Client).First();
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
                // TODO: Register new client as local or online
                //clients.Add(e.Client);

                // Informs the client that he is now known to the server
                e.Client.Send(OrderProvider.RegisterSuccessful);
            }
        }

        private void DisconnectClient(object sender, ServerEvent e)
        {
            if (IsKnown(e.Client))
            {
                // Get the match 
                var currentMatch = GetMatch(e.Client);

                if (currentMatch == null)
                {
                    // Remove the client from the match
                    registratedClientsPerGameTypeDict[currentMatch.GameType].Remove(e.Client);
                }
                else
                {
                    // Warn the opponent
                    var opponent = currentMatch.Client1 == e.Client ? currentMatch.Client1 : currentMatch.Client2;
                    opponent.Send(OrderProvider.OpponentDisconnected);
                }
            }
            else
            {
                Toolbox.LogError(new Exception("Client was not registred"));
            }
        }
    }
}
