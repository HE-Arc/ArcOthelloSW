using OthelloMillenniumClient.Classes.GameHandlers;
using System;
using Tools;

namespace OthelloMillenniumClient.Classes
{
    /// <summary>
    /// Current project must have a reference to OthelloMillenniumServer.dll
    /// </para>Bind HMI events to TCP messages
    /// </summary>
    public class ApplicationManager
    {
        #region Singleton
        private static object padlock = new object();
        private static ApplicationManager instance = null;

        public static ApplicationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        instance = new ApplicationManager();
                    }
                }
                return instance;
            }
        }

        #endregion

        private GameType gameType;
        private GameHandler gameHandler;

        private ApplicationManager() {}

        public void StartGameLocal(PlayerType playerType, BattleType battleType)
        {
            gameHandler = new LocalGameHandler(playerType, battleType);
            gameHandler.JoinGame();
        }

        public void StartGameOnline(BattleType battleType)
        {
            gameHandler = new OnlineGameHandler(battleType);
            gameHandler.JoinGame();

        }
    }
}
