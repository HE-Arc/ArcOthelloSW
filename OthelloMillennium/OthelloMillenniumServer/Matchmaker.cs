using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly HashSet<GameHandler> matches = new HashSet<GameHandler>();
        private readonly HashSet<Client> registratedClients = new HashSet<Client>();
        private readonly ConcurrentDictionary<Client, Container> searchingClients = new ConcurrentDictionary<Client, Container>();
        #endregion

        private void StartMatchmaking()
        {
            if (TCPServer.Instance.Running)
            {
                Task binderThread = new Task(async () =>
                {
                    while (TCPServer.Instance.Running)
                    {
                        lock (padlock)
                        {
                            List<Client> clientsToRemove = new List<Client>();
                            foreach (var kv in searchingClients)
                            {
                                Client client = kv.Key;
                                PlayerType opponentType = kv.Value.PlayerType;

                                if (SearchOpponent(client, opponentType) is Client opponent && searchingClients[opponent].PlayerType != PlayerType.None)
                                {
                                    // Start a new match
                                    StartNewMatch(client, opponent);

                                    // Remove the two from the registredClients
                                    clientsToRemove.Add(client);
                                    clientsToRemove.Add(opponent);

                                    // Switch opponentType to None in order to prevent binding with these
                                    // It has to be done like this since C# does not allow a modification on the list being iterated
                                    searchingClients[client].PlayerType = PlayerType.None;
                                    searchingClients[opponent].PlayerType = PlayerType.None;
                                }
                            }

                            // Remove clients
                            foreach (Client client in clientsToRemove)
                            {
                                searchingClients.TryRemove(client, out Container container);
                            }
                        }

                        // Sleep for 1 second
                        await Task.Delay(1000);
                    }
                });

                // Start the binding process
                binderThread.Start();
            }
            else
            {
                throw new Exception("Matchmaking already started !");
            }
        }

        /// <summary>
        /// Search for a suitable client
        /// </summary>
        /// <param name="client"></param>
        /// <returns>The opponent or null if no suitable opponent found</returns>
        private Client SearchOpponent(Client client, PlayerType opponentType)
        {
            if (opponentType == PlayerType.AI)
                return SearchAIOpponent(client);
            else
                return SearchPlayerOpponent(client);
        }

        /// <summary>
        /// Search for an AI
        /// <para/>(Different from the client if it is an ai)
        /// </summary>
        /// <param name="client"></param>
        /// <returns>The opponent or null if no suitable opponent found</returns>
        private Client SearchAIOpponent(OthelloTCPClient client)
        {
            return searchingClients.Keys.Where(ai => ai.PlayerType == PlayerType.AI).FirstOrDefault();
        }

        /// <summary>
        /// Search for a player
        /// <para/>(Different from the client if it is a player)
        /// </summary>
        /// <param name="client"></param>
        /// <returns>The opponent or null if no suitable opponent found</returns>
        private Client SearchPlayerOpponent(OthelloTCPClient client)
        {
            return searchingClients.Keys.Where(human => human.PlayerType == PlayerType.Human).FirstOrDefault();
        }

        private void StartNewMatch(Client client1, Client client2)
        {
            lock (padlock)
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
        public GameHandler GetMatch(OthelloTCPClient client)
        {
            return matches.Where(match => match.Client1 == client || match.Client2 == client).FirstOrDefault();
        }

        public bool IsKnown(OthelloTCPClient client)
        {
            return registratedClients.Contains(client) | searchingClients.Keys.Contains(client);
        }

        /// <summary>
        /// Register a new client and assign him his type and what he is searching for
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playerType"></param>
        /// <param name="opponentType"></param>
        /// <returns></returns>
        private bool Register(OthelloTCPClient othelloTCPClient, PlayerType playerType, string name)
        {
            lock (padlock)
            {
                if (!IsKnown(othelloTCPClient))
                {
                    Client client = new Client(playerType, name);

                    // React to searchOrder
                    client.OnOrderReceived += Client_OnOrderReceived;

                    // Bind the socket
                    client.Bind(othelloTCPClient.TcpClient);

                    // Add the client to the dictionnary
                    registratedClients.Add(client);

                    // Informs the client that he is now known to the server
                    client.Send(new RegisterSuccessfulOrder());

                    return true;
                }
            }
            Console.Error.WriteLine("Duplicate call for matchmaking with SingleClient");
            return false;
        }

        private void Client_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            lock (padlock)
            {
                if (sender is Client client && e.Order is SearchOrder order)
                {
                    if (IsKnown(client))
                    {
                        registratedClients.Remove(client);
                        searchingClients.TryAdd(client, new Container((PlayerType)order.OpponentType));

                        // Disconnect this function
                        client.OnOrderReceived -= Client_OnOrderReceived;
                    }
                    else
                    {
                        throw new Exception("Client unknown");
                    }
                }
            }
        }

        public bool RegisterNewClient(OthelloTCPClient client, Order order)
        {
            lock (padlock)
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

        private void DisconnectClient(Client client)
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
                        searchingClients.TryRemove(client, out Container container);
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
