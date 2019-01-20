using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<Client, PlayerType> searchingClients = new Dictionary<Client, PlayerType>();
        #endregion

        private void StartMatchmaking()
        {
            try
            {
                if (TCPServer.Instance.Running)
                {
                    Task binderThread = new Task(() =>
                    {
                        while (TCPServer.Instance.Running)
                        {
                            foreach (var kv in searchingClients)
                            {
                                Client client = kv.Key;
                                PlayerType opponentType = kv.Value;

                                if (SearchOpponent(client, opponentType) is Client opponent)
                                {
                                    // Start a new match
                                    StartNewMatch(client, opponent);

                                    // Remove the two from the registredClients
                                    searchingClients.Remove(client);
                                    searchingClients.Remove(opponent);
                                }
                            }

                            // Sleep for 1 second
                            Thread.Sleep(1000);
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
            catch (Exception ex)
            {
                Toolbox.LogError(ex);
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
            Console.WriteLine("Starting match");

            // Informs clients that an opponent has be found
            client1.Send(new OpponentFoundOrder(client2));
            client2.Send(new OpponentFoundOrder(client1));

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
            if (!IsKnown(othelloTCPClient))
            {
                Client client = new Client(playerType, name);
                registratedClients.Add(client);

                // React to searchOrder
                client.OnOrderReceived += Client_OnOrderReceived;

                // Informs the client that he is now known to the server
                client.Send(new RegisterSuccessfulOrder());

                return true;
            }
            else
            {
                return false;
            }
        }

        private void Client_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            if(sender is Client client && e.Order is SearchOrder order)
            {
                if(IsKnown(client))
                {
                    registratedClients.Remove(client);
                    searchingClients.Add(client, order.OpponentType);

                    // Disconnect this function
                    client.OnOrderReceived -= Client_OnOrderReceived;
                }
                else
                {
                    throw new Exception("Client unknown");
                }
            }
        }

        public bool RegisterNewClient(OthelloTCPClient client, Order order)
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
                    if(order is RegisterOrder rsOrder)
                    {
                        return Register(client, rsOrder.PlayerType, rsOrder.Name);
                    }
                    else if (order is LoadOrder loadOrder)
                    {
                        // Bypass the matchmaking process
                        //TODO
                    }
                }
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);
            }

            // It failed
            return false;
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
                        searchingClients.Remove(client);
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
