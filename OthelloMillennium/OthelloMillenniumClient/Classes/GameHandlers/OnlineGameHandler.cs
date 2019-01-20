using System;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumClient.Classes.GameHandlers
{
    public class OnlineGameHandler : GameHandler
    {
        public OnlineGameHandler(BattleType battleType)
        {
            GameType = GameType.Online;
            BattleType = battleType;
        }

        /// <summary>
        /// Register the client to the server
        /// <para/>Make it available through Player1
        /// </summary>
        /// <param name="client">Who to register</param>
        public override void Register(Client client)
        {
            base.Register(client);

            if (Player1 is null)
            {
                Player1 = client;
                Player1.OnGameStateReceived += GameStateUpdate;
            }
            else
            {
                throw new Exception("player has already been registred !");
            }
        }

        /// <summary>
        /// Start the matchmaking process
        /// </summary>
        public override void Search()
        {
            if (Player1 == null)
                throw new Exception("Please register player first");

            Player1.Search(BattleType);
        }
    }
}
