using System;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumClient.Classes.GameHandlers
{
    public abstract class GameHandler
    {
        #region Properties
        public GameType GameType { get; protected set; }
        public BattleType BattleType { get; protected set; }

        public GameState GameState { get; protected set; } = null;

        public Client Player1 { get; protected set; } = null;
        public Client Player2 { get; protected set; } = null;

        public event EventHandler<OthelloTCPClientArgs> OnGameReady;
        #endregion

        #region Abstract methods
        public virtual void Register(Client client)
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

        private void GameReadyReceived(object sender, OthelloTCPClientArgs e)
        {
            OnGameReady?.Invoke(this, e);
        }
        #endregion

        #region Methods
        public void DropClients()
        {
            Player1 = Player2 = null;
        }

        public Client GetCurrentPlayer()
        {
            return (Player1.CanPlay ? Player1 : Player2.CanPlay ? Player2 : null);
        }

        public void Place(Tuple<char, int> coords)
        {
            GetCurrentPlayer()?.Send(new PlayMoveOrder(coords));
        }
        #endregion
    }
}
