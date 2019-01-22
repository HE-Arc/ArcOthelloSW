using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tools;

namespace OthelloMillenniumServer
{
    // Used as a workaround
    internal class Container
    {
        public PlayerType PlayerType { get; set; }

        public Container(PlayerType playerType)
        {
            PlayerType = playerType;
        }
    }

    public class Matchmaker
    {
        #region Singleton
        private static readonly object padlock = new object();
        private static Matchmaker instance = null;

        public static Matchmaker Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Matchmaker();
                    }
                }
                return instance;
            }
        }

        private Matchmaker() {

            // Start matchmaking
            StartMatchmaking();
        }
        #endregion

        #region Attributes

        private readonly object registerLock = new object();

        private readonly HashSet<GameHandler> matches = new HashSet<GameHandler>();
        private readonly HashSet<Client_old> registratedClients = new HashSet<Client_old>();
        private readonly ConcurrentDictionary<Client_old, PlayerType> searchingClients = new ConcurrentDictionary<Client_old, PlayerType>();
        #endregion

        private void StartMatchmaking()
        {
            new Thread(() =>
            {
                while (TCPServer.Instance.Running)
                {
                    HashSet<Client_old> clientsToRemove = new HashSet<Client_old>();
                    foreach (var element in searchingClients)
                    {
                        Client_old client = element.Key;
                        PlayerType opponentType = element.Value;

                        if (SearchOpponent(client, opponentType) is Client_old opponent && !clientsToRemove.Contains(opponent))
                        {
                            // Start a new match
                            StartNewMatch(client, opponent);

                            // Remove the two from the registredClients
                            clientsToRemove.Add(client);
                            clientsToRemove.Add(opponent);
                        }
                    }

                    // Remove clients
                    while(clientsToRemove.Count > 0)
                    {
                        Client_old client = clientsToRemove.First();
                        searchingClients.TryRemove(client, out PlayerType playerType);
                        clientsToRemove.Remove(client);
                    }
                }
            }).Start();
        }

        /// <summary>
        /// Search for a suitable client
        /// </summary>
        /// <param name="client"></param>
        /// <returns>The opponent or null if no suitable opponent found</returns>
        private Client_old SearchOpponent(Client_old client, PlayerType opponentType)
        {
            if (searchingClients.Keys.Where(ai => ai.PlayerType == opponentType).Where(c => c != client).Count() == 0)
            {
                return null;
            }
            return searchingClients.Keys.Where(ai => ai.PlayerType == opponentType).Where(c => c != client).FirstOrDefault();
        }

        private void StartNewMatch(Client_old client1, Client_old client2)
        {
            // Informs clients that an opponent has be found
            client1.Send(new OpponentFoundOrder(client2.Name));
            client2.Send(new OpponentFoundOrder(client1.Name));

            // GameManager will now handle clients and put them as InGame
            var match = new GameHandler(client1, client2);
            matches.Add(match);

            // Link end of game event
            match.GameManager.OnGameFinished += GameManager_OnGameFinished;
        }

        private void GameManager_OnGameFinished(object sender, GameState e)
        {
            if(sender is GameManager match)
            {
                // Unsubscribe
                match.OnGameFinished -= GameManager_OnGameFinished;

                // TODO: Inform clients
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the match in which the player plays
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public GameHandler GetMatch(OthelloTCPClient_old client)
        {
            return matches.Where(match => match.Client1 == client || match.Client2 == client).FirstOrDefault();
        }

        public bool IsKnown(OthelloTCPClient_old client)
        {
            return registratedClients.Contains(client) || searchingClients.Keys.Contains(client);
        }

        /// <summary>
        /// Register a new client and assign him his type and what he is searching for
        /// <para/>Locked by the calling method RegisterNewClient
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playerType"></param>
        /// <param name="opponentType"></param>
        /// <returns></returns>
        private bool Register(OthelloTCPClient_old othelloTCPClient, PlayerType playerType, string name)
        {
            if (!IsKnown(othelloTCPClient))
            {
                Client_old client = new Client_old(playerType, name);

                // React to searchOrder
                othelloTCPClient.OnOrderReceived += SearchReceived;

                // NOTE : Event seems to not raise across herited classes ... 

                // Bind the socket
                client.Bind(othelloTCPClient.TcpClient);

                // Add the client to the dictionnary
                registratedClients.Add(client);

                // Informs the client that he is now known to the server
                client.Send(new RegisterSuccessfulOrder());

                return true;
            }
            else
            {
                Console.Error.WriteLine("Duplicate call for matchmaking with SingleClient");
                return false;
            }
        }

        private Client_old GetClientFromSender(object sender)
        {
            var tmp = sender as OthelloTCPClient_old;
            return registratedClients.Union(searchingClients.Keys).Where(x => x.TcpClient.Equals(tmp.TcpClient)).FirstOrDefault();
        }

        private void SearchReceived(object sender, OthelloTCPClientArgs e)
        {
            if (GetClientFromSender(sender) is Client_old client && e.Order is SearchRequestOrder order)
            {
                if (IsKnown(client))
                {
                    registratedClients.Remove(client);
                    searchingClients.TryAdd(client,(PlayerType)order.OpponentType);

                    // Disconnect this function
                    client.OnOrderReceived -= SearchReceived;
                }
                else
                {
                    throw new Exception("Client unknown");
                }
            }
        }

        public bool RegisterNewClient(OthelloTCPClient client, Order order)
        {
            lock (registerLock)
            {
                try
                {
                    if (IsKnown(client))
                    {
                        var currentMatch = GetMatch(client);
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
                            Console.WriteLine("Client will be reconnected to the match"); // DEBUG
                        }
                    }
                    else
                    {
                        // Store the new client in the matching hashset according to the player's game type wish
                        if (order is RegisterRequestOrder rsOrder)
                        {
                            return Register(client, (PlayerType)rsOrder.PlayerType, rsOrder.Name);
                        }
                        else if (order is LoadOrder loadOrder)
                        {
                            Console.WriteLine("By passing matchmaking process in order to load a game !");
                            // Bypass the matchmaking process
                            //TODO
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while registring client");
                    Toolbox.LogError(ex);
                }

                // It failed
                Console.Error.WriteLine("Error during RegisterNewClient order");
                return false;
            }
        }

        private void DisconnectClient(Client_old client)
        {
            try
            {
                if (IsKnown(client))
                {
                    // Get the match
                    var currentMatch = GetMatch(client);

                    if (currentMatch == null)
                    {
                        // Remove the client from the matchmaking
                        registratedClients.Remove(client);
                        searchingClients.TryRemove(client, out PlayerType playerType);
                    }
                    else
                    {

                        // Warn the opponent
                        if(currentMatch.Client1.Equals(client))
                        {
                            currentMatch.Client2.Send(new OpponentDisconnectedOrder());
                        }
                        else if (currentMatch.Client2.Equals(client))
                        {
                            currentMatch.Client1.Send(new OpponentDisconnectedOrder());
                        }
                        else
                        {
                            throw new Exception("Opponent not found");
                        }
                    }
                }
                else
                {
                    Toolbox.LogError(new Exception("Client was not registred"));
                }
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);
            }
        }
    }
}
