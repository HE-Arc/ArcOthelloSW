using OthelloMillenniumClient.Classes.GameHandlers;
using System;

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

        private ApplicationManager() {
            int port = StartLocalServer();
 
            // Test if server has been started
            if (port < 0)
            {
                throw new Exception($"Unable to start server on port {port}");
            }

        }
        #endregion

        public GameHandler CurrentGame { get; set; }

        public Client Player1 => CurrentGame.Player1;
        public Client Player2 => CurrentGame.Player2;

        #region Methods
        /// <summary>
        /// Start a local server
        /// </summary>
        /// <returns>Port where server is listening or -1 if it failed</returns>
        private int StartLocalServer() {
            try
            {
                int port = (new Random()).Next(49152, 65535);
                OthelloMillenniumServer.TCPServer.Port = port;
                OthelloMillenniumServer.TCPServer.Instance.StartListening();
                return port;
            }
            catch(Exception ex)
            {
                OthelloMillenniumServer.Toolbox.LogError(ex);
                return -1;
            }
        }

        public void SetCurrentHMI() { }
        #endregion
    }
}
