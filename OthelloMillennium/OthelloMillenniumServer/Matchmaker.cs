using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumServer
{
    internal class LoadRequest
    {
        public OthelloPlayerServer Player1 { get; private set; }
        public OthelloPlayerServer Player2 { get; private set; }
        public SaveFile SaveFile { get; private set; }

        public LoadRequest(OthelloPlayerServer emitter, SaveFile savefile)
        {
            Player1 = emitter;
            SaveFile = savefile;
        }

        public void AssignOpponent(OthelloPlayerServer opponent)
        {
            Player2 = opponent;

            // Inform the client
            Player2.JoinSuccessful();
        }
    }

    // Used as a workaround
    internal class Container
    {
        public PlayerType PlayerType { get; set; }

        public Container(PlayerType playerType)
        {
            PlayerType = playerType;
        }
    }

    public class Matchmaker : IOrderHandler
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
        private readonly HashSet<OthelloPlayerServer> registratedClients = new HashSet<OthelloPlayerServer>();
        private readonly ConcurrentDictionary<int, LoadRequest> loadedGames = new ConcurrentDictionary<int, LoadRequest>();
        private readonly ConcurrentDictionary<OthelloPlayerServer, PlayerType> searchingClients = new ConcurrentDictionary<OthelloPlayerServer, PlayerType>();
        #endregion

        private void StartMatchmaking()
        {
            new Task(() =>
            {
                while (TCPServer.Instance.Running)
                {
                    HashSet<OthelloPlayerServer> clientsToRemove = new HashSet<OthelloPlayerServer>();
                    foreach (var element in searchingClients)
                    {
                        OthelloPlayerServer client = element.Key;
                        PlayerType opponentType = element.Value;

                        if (SearchOpponent(client, opponentType) is OthelloPlayerServer opponent && !clientsToRemove.Contains(opponent))
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
                        OthelloPlayerServer client = clientsToRemove.First();
                        searchingClients.TryRemove(client, out PlayerType playerType);
                        clientsToRemove.Remove(client);
                    }

                    Thread.Sleep(10);
                }
            }).Start();
        }

        /// <summary>
        /// Search for a suitable client
        /// </summary>
        /// <param name="othelloPlayer"></param>
        /// <returns>The opponent or null if no suitable opponent found</returns>
        private OthelloPlayerServer SearchOpponent(OthelloPlayerServer othelloPlayer, PlayerType opponentType)
        {
            if (searchingClients.Keys.Where(ai => ai.PlayerType == opponentType).Where(c => c != othelloPlayer).Count() == 0)
            {
                return null;
            }
            return searchingClients.Keys.Where(ai => ai.PlayerType == opponentType).Where(c => c != othelloPlayer).FirstOrDefault();
        }

        private void StartNewMatch(OthelloPlayerServer othelloPlayer1, OthelloPlayerServer othelloPlayer2)
        {
            // Informs clients that an opponent has be found
            othelloPlayer1.OpponentFound(othelloPlayer2.Name, othelloPlayer2.PlayerType);
            othelloPlayer2.OpponentFound(othelloPlayer1.Name, othelloPlayer1.PlayerType);

            // GameManager will now handle clients and put them as InGame
            CreateMatch(othelloPlayer1, othelloPlayer2).Init();
        }

        private void StartNewMatch(LoadRequest loadRequest)
        {
            // Informs clients that an opponent has be found
            loadRequest.Player1.OpponentFound(loadRequest.Player2.Name, loadRequest.Player2.PlayerType);
            loadRequest.Player2.OpponentFound(loadRequest.Player1.Name, loadRequest.Player1.PlayerType);

            // GameManager will now handle clients and put them as InGame
            CreateMatch(loadRequest.Player1, loadRequest.Player2).Load(loadRequest.SaveFile);
        }

        /// <summary>
        /// GameManager will now handle clients and put them as InGame
        /// </summary>
        /// <param name="othelloPlayer1"></param>
        /// <param name="othelloPlayer2"></param>
        private GameHandler CreateMatch(OthelloPlayerServer othelloPlayer1, OthelloPlayerServer othelloPlayer2)
        {
            GameHandler match;
            if (new Random().Next() % 2 == 0)
            {
                match = new GameHandler(othelloPlayer1, othelloPlayer2);
            }
            else
            {
                match = new GameHandler(othelloPlayer2, othelloPlayer1);
            }

            // Store the match
            matches.Add(match);

            return match;
        }

        /// <summary>
        /// Retrieves the match in which the player plays
        /// </summary>
        /// <param name="othelloPlayer"></param>
        /// <returns>Match in which the player plays</returns>
        public GameHandler GetMatch(OthelloPlayerServer othelloPlayer)
        {
            return matches.Where(match => match.OthelloPlayer1 == othelloPlayer || match.OthelloPlayer2 == othelloPlayer).FirstOrDefault();
        }

        public bool IsKnown(OthelloPlayerServer client)
        {
            return registratedClients.Contains(client) || searchingClients.Keys.Contains(client);
        }

        /// <summary>
        /// Register a new client
        /// </summary>
        /// <param name="othelloPlayer"></param>
        /// <returns></returns>
        private bool Register(OthelloPlayerServer othelloPlayer)
        {
            if (!IsKnown(othelloPlayer))
            {
                // Add the client to the dictionnary
                registratedClients.Add(othelloPlayer);

                // Informs the client that he is now known to the server
                othelloPlayer.RegisterSuccessful(othelloPlayer.Name, othelloPlayer.PlayerType);

                return true;
            }
            else
            {
                Console.Error.WriteLine("Duplicate call for matchmaking with SingleClient");
                return false;
            }
        }

        /// <summary>
        /// Matchmake a client
        /// </summary>
        /// <param name="othelloTCPClient"></param>
        /// <returns></returns>
        private bool Matchmake(OthelloPlayerServer othelloPlayer, PlayerType opponentType)
        {
            if (IsKnown(othelloPlayer))
            {
                registratedClients.Remove(othelloPlayer);
                searchingClients.TryAdd(othelloPlayer, opponentType);

                return true;
            }
            else
            {
                Console.Error.WriteLine("Duplicate call for matchmaking with SingleClient");
                return false;
            }
        }

        private bool RegisterGame(OthelloPlayerServer emitter, SaveFile saveFile)
        {
            // Test if the player is known
            if(!IsKnown(emitter)) throw new Exception("Unknown client");

            // Generate the "savefile"
            LoadRequest loadRequest = new LoadRequest(emitter, saveFile);
            int requestID = loadRequest.GetHashCode();

            // Verifiy that the game does not exist
            if (loadedGames.ContainsKey(requestID)) throw new Exception("Game already loaded");

            // Add it to the list
            loadedGames.TryAdd(requestID, loadRequest);

            // Send the id to the emitter
            emitter.LoadSuccessful(requestID);

            return true;
        }

        private bool JoinGame(OthelloPlayerServer emitter, int gameID)
        {
            // Test if the player is known
            if (!IsKnown(emitter)) throw new Exception("Unknown client");

            // Try to get the loadRequest
            if(loadedGames.TryGetValue(gameID, out LoadRequest loadRequest))
            {
                // Assign an opponent
                loadRequest.AssignOpponent(emitter);

                // Start a new match
                StartNewMatch(loadRequest);

                // Remove the match
                loadedGames.TryRemove(gameID, out LoadRequest dump);

                // Success
                return true;
            }

            // Failure
            return false;
        }

        public void SetOrderHandler(IOrderHandler handler)
        {
            throw new Exception("This object can not receive an handler");
        }

        public void HandleOrder(IOrderHandler sender, Order order)
        {
            // If null, sender is this object otherwise the order has been redirected
            sender = sender ?? this;
            OthelloPlayerServer castedSender = sender as OthelloPlayerServer;

            switch (order)
            {
                case RegisterRequestOrder castedOrder:
                    Register(castedSender);
                    break;

                case SearchRequestOrder castedOrder:
                    // Look for the sender
                    Matchmake(castedSender, (PlayerType)castedOrder.OpponentType);
                    break;

                case LoadRequestOrder castedOrder:
                    RegisterGame(castedSender, castedOrder.SaveFile);
                    break;

                case JoinRequestOrder castedOrder:
                    JoinGame(castedSender, castedOrder.GameID);
                    break;
            }
        }

        private void DisconnectClient(OthelloPlayerServer client)
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
                        if(currentMatch.OthelloPlayer1.Equals(client))
                        {
                            currentMatch.OthelloPlayer2.OpponentDisconnected();
                        }
                        else if (currentMatch.OthelloPlayer2.Equals(client))
                        {
                            currentMatch.OthelloPlayer1.OpponentDisconnected();
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
