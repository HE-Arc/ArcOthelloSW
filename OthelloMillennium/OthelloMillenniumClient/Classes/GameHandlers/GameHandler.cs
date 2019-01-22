using System;
using Tools;

namespace OthelloMillenniumClient.Classes.GameHandlers
{
    public abstract class GameHandler : OrderHandler
    {
        #region Properties
        public GameType GameType { get; protected set; }

        public GameState GameState { get; protected set; } = null;
        public Client_old Player1 { get; protected set; } = null;
        public Client_old Player2 { get; protected set; } = null;

        public event EventHandler<OthelloTCPClientArgs> OnGameReady;

        private int gameReadyReceivedCount = 0;
        
        #endregion


        public GameHandler()
        {
            //Nothing
        }

        public abstract void JoinGame();
        public abstract void StartGame();


        #region Abstract methods
        public virtual void Register(Client_old client)
        {
            // Register client to the server
            client.Register(GameType);
            client.OnGameReadyReceived += GameReadyReceived;
            client.OnGameStateReceived += GameStateUpdate;
        }
        public abstract void Search();
        #endregion

        #region Events
        protected void GameStateUpdate(object sender, OthelloTCPClientGameStateArgs e)
        {
            GameState = e.GameState;
        }

        protected void GameReadyReceived(object sender, OthelloTCPClientArgs e)
        {
            if(GameType == GameType.Online)
                OnGameReady?.Invoke(this, e);
            else
            {
                if(gameReadyReceivedCount > 0)
                    OnGameReady?.Invoke(this, e);
            }

            gameReadyReceivedCount += 1;
        }
        #endregion
        
        public abstract void HandleOrder(Order order);
    }
}
