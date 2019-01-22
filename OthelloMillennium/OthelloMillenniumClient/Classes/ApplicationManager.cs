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
        private static readonly object padlock = new object();
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

        private ApplicationManager() {}
        #endregion

        public GameHandler CurrentGame { get; set; }

        public Client_old Player1 => CurrentGame.Player1;
        public Client_old Player2 => CurrentGame.Player2;

        #region Methods

        public void SetCurrentHMI() { }
        #endregion
    }
}
