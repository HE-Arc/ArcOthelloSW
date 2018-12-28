using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly HashSet<ManagedClient> handledClients = new HashSet<ManagedClient>();
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
                        var waitingClients = handledClients.Where(client => client.State == ManagedClient.ManagedState.Searching).ToHashSet();
                        if (waitingClients.Count > 2)
                        {
                            // TODO : Insert logic (i.e. ranking, waiting time, etc)

                            // For now we take the first and the last one
                            var client1 = waitingClients.First();
                            var client2 = waitingClients.Last();

                            // Bind clients
                            ManagedClient.Bind(client1, client2);

                            // GameManager will now handle clients and put them as InGame
                            matches.Add(new GameHandler(client1, client2));
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
            ManagedClient newClient = new ManagedClient(e.Client);

            if (handledClients.TryGetValue(newClient, out ManagedClient outputClient))
            {
                switch (outputClient.State)
                {
                    case ManagedClient.ManagedState.Searching:
                        throw new Exception("Client already registred and searching for a game");
                    case ManagedClient.ManagedState.Binded:
                        throw new Exception("Client already registred and bindind with an opponent");
                    case ManagedClient.ManagedState.InGame:
                        
                        //TODO: handle reconnection to the game
                        break;
                    case ManagedClient.ManagedState.Disconnected:
                        //GameHandler match = matches.Where(gh => gh.client1.Equals(outputClient)).First();
                        //match.OnClientReconnected?.Invoke(this, new GameHandlerArgs() { Client = outputClient });
                        break;
                }
            }
            else
            {
                // Register new client
                handledClients.Add(newClient);
            }
        }

        private void DisconnectClient(object sender, ServerEvent e)
        {
            ManagedClient newClient = new ManagedClient(e.Client);

            if (handledClients.TryGetValue(newClient, out ManagedClient outputClient))
            {
                switch (outputClient.State)
                {
                    case ManagedClient.ManagedState.Disconnected:
                    case ManagedClient.ManagedState.Searching:
                        break;

                    case ManagedClient.ManagedState.Binded:
                        // outputClient.Opponent. <---------------------------- FIXME
                        // TODO : make binded client search for a new game
                        break;
                    case ManagedClient.ManagedState.InGame:
                        // TODO : handle disconnect from the game
                        break;
                }

                // Switch user to disconnected
                //outputClient.State = ManagedState.Disconnected;

                // Remove client from the dictionnary
                handledClients.Remove(outputClient);
            }
            else
            {
                throw new Exception("Client is not registred");
            }
        }
    }
}
