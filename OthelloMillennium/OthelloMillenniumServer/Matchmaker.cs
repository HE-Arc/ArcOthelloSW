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
            // Init dictionnary
            registratedClientsPerGameTypeDict.Add(GameManager.GameType.SinglePlayer, new HashSet<OthelloTCPClient>());
            registratedClientsPerGameTypeDict.Add(GameManager.GameType.MultiPlayer, new HashSet<OthelloTCPClient>());

            // Start matchmaking
            StartMatchmaking();
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

        private void StartPinging()
        {
            Task pinger = new Task(() =>
            {
                foreach (var set in registratedClientsPerGameTypeDict.Values)
                {
                    foreach (var client in set)
                    {
                        if (!Toolbox.Connected(client))
                        {
                            DisconnectClient(client);
                        }
                    }
                }

                Thread.Sleep(5000);
            });

            pinger.Start();
        }

        private void StartMatchmaking()
        {
            try
            {
                if (!running)
                {
                    running = true;
                    Task binderThread = new Task(() =>
                    {
                        while (running)
                        {
                            Console.WriteLine("Currently queuing");
                            Console.WriteLine($"Online  : {OnlineClients.Count}");
                            Console.WriteLine($"Local  : {LocalClients.Count}");

                            #region Local
                            // Start to look for any good binding if there is 2 or more player/AI waiting
                            if (LocalClients.Count > 1)
                            {
                                StartNewMatch(GameManager.GameType.SinglePlayer);
                            }
                            #endregion

                            #region Online
                            // Start to look for any good binding if there is 2 or more player/AI waiting
                            if (OnlineClients.Count > 1)
                            {
                                StartNewMatch(GameManager.GameType.MultiPlayer);
                            }
                            #endregion

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

        private void StartNewMatch(GameManager.GameType gameType)
        {
            Console.WriteLine("Starting match");

            // Get the corresponding set
            var set = gameType == GameManager.GameType.MultiPlayer ? OnlineClients : LocalClients;

            // TODO : Insert logic (i.e. ranking, waiting time, etc)

            // For now we take the first and the last one
            var client1 = set.First();
            var client2 = set.Last();

            // GameManager will now handle clients and put them as InGame
            var match = new GameHandler(client1, client2, gameType);
            matches.Add(match);

            // Link end of game event
            match.GameManager.OnGameFinished += GameManager_OnGameFinished;

            // Informs clients that an opponent has be found
            client1.Send(OrderProvider.OpponentFound);
            client2.Send(OrderProvider.OpponentFound);

            // Update client state
            client1.State = PlayerState.Binded;
            client2.State = PlayerState.Binded;

            // Remove them from the queue
            set.Remove(client1);
            set.Remove(client2);
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
            return LocalClients.Contains(client) | OnlineClients.Contains(client);
        }

        private bool Register(OthelloTCPClient client, GameManager.GameType gameType)
        {
            bool result = registratedClientsPerGameTypeDict[gameType].Add(client);
            if (result)
            {
                // Informs the client that he is now known to the server
                client.Send(OrderProvider.RegisterSuccessful);

                // Update client state
                client.State = PlayerState.Searching;
            }
            else
            {
                throw new Exception("Error while trying to register new client");
            }
            return result;
        }

        public bool RegisterNewClient(OthelloTCPClient client, AOrder order)
        {
            try
            {
                if (IsKnown(client))
                {
                    var currentMatch = matches.Where(match => match.Client1 == client || match.Client2 == client).First();
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
                        client.State = PlayerState.Undefined; // TODO
                    }
                }
                else
                {
                    // Store the new client in the matching hashset according to the player's game type wish
                    switch (order)
                    {
                        case SearchLocalGameOrder x1:
                            return Register(client, GameManager.GameType.SinglePlayer);
                        case SearchOnlineGameOrder x2:
                            return Register(client, GameManager.GameType.MultiPlayer);
                        default:
                            throw new Exception("Invalid order received");
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

                    // Mark client as disconnected
                    client.State = PlayerState.Disconnected;

                    if (currentMatch == null)
                    {
                        // Remove the client from the matchmaking
                        Console.WriteLine("Disconnected");
                        registratedClientsPerGameTypeDict[currentMatch.GameType].Remove(client);
                    }
                    else
                    {
                        // Warn the opponent
                        if(client.Properties.TryGetValue("Opponent", out object output) && output is OthelloTCPClient opponent)
                        {
                            opponent.Send(OrderProvider.OpponentDisconnected);
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
