using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumClient.Classes.GameHandlers
{
    public class LocalGameHandler : GameHandler
    {
        public LocalGameHandler()
        {
            GameType = GameType.Local;
        }

        /// <summary>
        /// Register the client to the server
        /// <para/>Make it available through Player1 or Player2
        /// </summary>
        /// <param name="client">Who to register</param>
        public override void Register(Client client)
        {
            // Register client to the server
            client.Register(GameType);

            if (Player1 is null)
            {
                Player1 = client;
                Player1.OnGameStateReceived += GameStateUpdate;
            }
            else if (Player2 is null)
            {
                Player2 = client;
                Player2.OnGameStateReceived += GameStateUpdate;
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
    }
}
