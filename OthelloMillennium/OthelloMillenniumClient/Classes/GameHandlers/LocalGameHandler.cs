using OthelloMillenniumServer;
using System;
using Tools;

namespace OthelloMillenniumClient.Classes.GameHandlers
{
    public class LocalGameHandler : GameHandler
    {
        public LocalGameHandler()
        {
            GameType = GameType.Local;

            // Test if server has been started
            if (StartLocalServer() < 0)
            {
                throw new Exception($"Unable to start server on port {TCPServer.Instance.Port}");
            }
        }

        /// <summary>
        /// Register the client to the server
        /// <para/>Make it available through Player1 or Player2
        /// </summary>
        /// <param name="client">Who to register</param>
        public override void Register(Client client)
        {
            base.Register(client);

            if (Player1 is null)
            {
                Player1 = client;
            }
            else if (Player2 is null)
            {
                Player2 = client;
            }
            else
            {
                throw new Exception("2 players have already been registred !");
            }
        }

        /// <summary>
        /// Start the matchmaking process
        /// </summary>
        public override void Search()
        {
            if (Player1 == null | Player2 == null)
                throw new Exception("Please register players first");

            if (Player1.PlayerType == PlayerType.Human)
            {
                if(Player2.PlayerType == PlayerType.Human)
                {
                    Player1.Search(BattleType.AgainstPlayer);
                    Player2.Search(BattleType.AgainstPlayer);
                    BattleType = BattleType.AgainstPlayer;
                }
                else
                {
                    Player1.Search(BattleType.AgainstAI);
                    Player2.Search(BattleType.AgainstPlayer);
                    BattleType = BattleType.AgainstAI;
                }
            }
            else
            {
                if (Player2.PlayerType == PlayerType.Human)
                {
                    Player1.Search(BattleType.AgainstPlayer);
                    Player2.Search(BattleType.AgainstAI);
                }
                else
                {
                    Player1.Search(BattleType.AgainstAI);
                    Player2.Search(BattleType.AgainstAI);
                }
                BattleType = BattleType.AgainstAI;
            }
        }

        /// <summary>
        /// Start a local server
        /// </summary>
        /// <returns>Port where server is listening or -1 if it failed</returns>
        private int StartLocalServer()
        {
            try
            {
                int port = (new Random()).Next(49152, 65535);
                TCPServer.Instance.Port = port;
                TCPServer.Instance.StartListening();
                return port;
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);
                return -1;
            }
        }
    }
}
