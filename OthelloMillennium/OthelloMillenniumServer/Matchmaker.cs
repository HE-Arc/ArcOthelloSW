using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OthelloMillenniumServer.Client;

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
        private readonly HashSet<GameManager> matches = new HashSet<GameManager>();
        private readonly HashSet<Client> handledClients = new HashSet<Client>();
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
                        var waitingClients = handledClients.Where(client => client.State == ManagedState.Searching).ToHashSet();
                        if (waitingClients.Count > 2)
                        {
                            // TODO : Insert logic (i.e. ranking, waiting time, etc)

                            // For now we take the first and the last one
                            var client1 = waitingClients.First();
                            var client2 = waitingClients.Last();

                            // Bind clients
                            client1.State = client2.State = ManagedState.Binded;

                            // GameManager will now handle clients and put them as InGame
                            matches.Add(new GameManager(client1, client2));
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
            if (handledClients.TryGetValue(e.Client, out Client outputClient))
            {
                switch (outputClient.State)
                {
                    case ManagedState.Searching:
                        throw new Exception("Client already registred and searching for a game");
                    case ManagedState.Binded:
                        throw new Exception("Client already registred and bindind with an opponent");
                    case ManagedState.InGame:
                        //TODO: handle reconnection to the game
                        break;
                    case ManagedState.Disconnected:
                        //TODO: notify him with a message ?
                        outputClient.Register();
                        break;
                }
            }
            else
            {
                // Register new client
                handledClients.Add(e.Client.Register());
            }
        }

        private void DisconnectClient(object sender, ServerEvent e)
        {
            if (handledClients.TryGetValue(e.Client, out Client outputClient))
            {
                switch (outputClient.State)
                {
                    case ManagedState.Disconnected:
                    case ManagedState.Searching:
                        break;

                    case ManagedState.Binded:
                        // TODO : make binding client search for a new game
                        break;
                    case ManagedState.InGame:
                        // TODO : handle disconnect to the game
                        break;
                }

                // Switch user to disconnected
                outputClient.State = ManagedState.Disconnected;

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
