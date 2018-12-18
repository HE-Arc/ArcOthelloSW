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
        public enum State
        {
            Searching,
            Binded,
            InGame,
            Disconnected
        }

        public class ClientInformation
        {
            public ClientInformation(DateTime registerTime)
            {
                State = State.Searching;
                RegisterDateTime = registerTime;
            }

            public State State { get; set; }
            public DateTime RegisterDateTime { get; private set; }
            public DateTime LastValidPingDateTime { get; set; }
        }

        public class WrappedClient
        {
            public WrappedClient(Client client, ClientInformation informations)
            {
                Client = client;
                Informations = informations;
            }

            public Client Client { get; private set; }
            public ClientInformation Informations { get; private set; }
        }

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
        private readonly HashSet<object> matches = new HashSet<object>(); //FIXME : should be GameManager and not an object
        private readonly Dictionary<Client, ClientInformation> handledClients = new Dictionary<Client, ClientInformation>();
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
                        var waitingClients = handledClients.Where(kv => kv.Value.State == State.Searching).ToDictionary(kv => kv.Key, kv => kv.Value);
                        if (waitingClients.Count > 2)
                        {
                            // TODO : Insert logic (i.e. ranking, waiting time, etc)

                            // For now we take the first and the last one
                            var client1KV = waitingClients.First();
                            var client2KV = waitingClients.Last();

                            // Extract data
                            Client client1 = client1KV.Key;
                            Client client2 = client2KV.Key;

                            ClientInformation client1Informations = client1KV.Value;
                            ClientInformation client2Informations = client2KV.Value;

                            // Bind clients
                            client1Informations.State = client2Informations.State = State.Binded;

                            // GameManager will now handle clients and put them as InGame
                            // client1Informations.State = client2Informations.State = State.InGame;
                            matches.Add(StartMatch(new WrappedClient(client1, client1Informations), new WrappedClient(client2, client2Informations)));
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

        /// <summary>
        /// TODO: change return type to gameManager
        /// </summary>
        private object StartMatch(WrappedClient wrappedClient1, WrappedClient wrappedClient2)
        {
            // Must subscribe to OnGameFinished of GameManager
            return null;
        }

        private void RegisterNewClient(object sender, ServerEvent e)
        {
            if (handledClients.TryGetValue(e.Client, out ClientInformation clientInformation))
            {
                switch (clientInformation.State)
                {
                    case State.Searching:
                        throw new Exception("Client already registred and searching for a game");
                    case State.Binded:
                        throw new Exception("Client already registred and bindind with an opponent");
                    case State.InGame:
                        //TODO: handle reconnection to the game
                        break;
                    case State.Disconnected:
                        //TODO: notify him with a message ?
                        handledClients.Add(e.Client, new ClientInformation(e.FiredDateTime));
                        break;
                }
            }
            else
            {
                // Register new client
                handledClients.Add(e.Client, new ClientInformation(e.FiredDateTime));
            }
        }

        private void DisconnectClient(object sender, ServerEvent e)
        {
            if (handledClients.TryGetValue(e.Client, out ClientInformation clientInformation))
            {
                switch (clientInformation.State)
                {
                    case State.Disconnected:
                    case State.Searching:
                        break;

                    case State.Binded:
                        // TODO : make binding client search for a new game
                        break;
                    case State.InGame:
                        // TODO : handle disconnect to the game
                        break;
                }

                // Switch user to disconnected
                clientInformation.State = State.Disconnected;

                // Remove client from the dictionnary
                handledClients.Remove(e.Client);
            }
            else
            {
                throw new Exception("Client is not registred");
            }
        }
    }
}
