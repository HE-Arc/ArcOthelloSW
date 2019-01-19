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
            // Init dictionnary
            registratedClients.Add(PlayerType.AI, new HashSet<OthelloTCPClient>());
            registratedClients.Add(PlayerType.Human, new HashSet<OthelloTCPClient>());

            // Start matchmaking
            StartMatchmaking();
        }
        #endregion

        #region Attributes
        private readonly HashSet<GameHandler> matches = new HashSet<GameHandler>();
        private readonly Dictionary<PlayerType, HashSet<OthelloTCPClient>> registratedClients = new Dictionary<PlayerType, HashSet<OthelloTCPClient>>();
        #endregion

        #region Properties
        HashSet<OthelloTCPClient> RegistratedAIs => registratedClients[PlayerType.AI];
        HashSet<OthelloTCPClient> RegistratedPlayers => registratedClients[PlayerType.Human];
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
                            foreach (var client in RegistratedPlayers.Union(RegistratedAIs).Where(client => (bool)client.Properties["Active"]))
                            {
                                if (SearchOpponent(client) is OthelloTCPClient opponent)
                                {
                                    // Start a new match
                                    StartNewMatch(client, opponent);

                                    // Remove the two from the registredClients
                                    ((PlayerType)client.Properties["PlayerType"] == PlayerType.Human ?  RegistratedPlayers : RegistratedAIs).Remove(client);
                                    ((PlayerType)opponent.Properties["PlayerType"] == PlayerType.Human ? RegistratedPlayers : RegistratedAIs).Remove(opponent);
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
        private OthelloTCPClient SearchOpponent(OthelloTCPClient client)
        {
            if ((PlayerType)client.Properties["Searching"] == PlayerType.AI)
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
        private OthelloTCPClient SearchAIOpponent(OthelloTCPClient client)
        {
            return RegistratedAIs.Where(ai => !ai.Equals(client)).FirstOrDefault();
        }

        /// <summary>
        /// Search for a player
        /// <para/>(Different from the client if it is a player)
        /// </summary>
        /// <param name="client"></param>
        /// <returns>The opponent or null if no suitable opponent found</returns>
        private OthelloTCPClient SearchPlayerOpponent(OthelloTCPClient client)
        {
            return RegistratedPlayers.Where(player => !player.Equals(client)).FirstOrDefault();
        }

        private void StartNewMatch(OthelloTCPClient client1, OthelloTCPClient client2)
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
            return RegistratedAIs.Contains(client) | RegistratedPlayers.Contains(client);
        }

        /// <summary>
        /// Register a new client and assign him his type and what he is searching for
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playerType"></param>
        /// <param name="opponentType"></param>
        /// <returns></returns>
        private bool Register(OthelloTCPClient client, PlayerType playerType, string Name)
        {
            // Try to add the client
            bool result = registratedClients[playerType].Add(client);

            // if result == false, client already exist
            if (result)
            {
                // Set client properties
                client.Properties.Add("Name", Name);
                client.Properties.Add("PlayerType", playerType);
                client.Properties.Add("Active", false);

                // React to searchOrder
                client.OnOrderReceived += Client_OnOrderReceived;

                // Informs the client that he is now known to the server
                client.Send(new RegisterSuccessfulOrder());
            }
            else
            {
                throw new Exception("Error while trying to register new client");
            }
            return result;
        }

        private void Client_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            if(sender is OthelloTCPClient client && e.Order is SearchOrder order)
            {
                // Register the type of player the client is searching
                client.Properties.Add("Searching", order.OpponentType);

                // Make him active for matchmaking
                client.Properties["Active"] = true;

                // Disconnect this function
                client.OnOrderReceived -= Client_OnOrderReceived;
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

        private void DisconnectClient(OthelloTCPClient client)
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
                        registratedClients[(PlayerType)client.Properties["PlayerType"]].Remove(client);
                    }
                    else
                    {
                        // Warn the opponent
                        if(client.Properties.TryGetValue("Opponent", out object output) && output is OthelloTCPClient opponent)
                        {
                            opponent.Send(new OpponentDisconnectedOrder());
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
