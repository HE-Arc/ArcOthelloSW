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
        public bool IsReady { get; protected set; } = false;

        public Client Player1 { get; protected set; } = null;
        public Client Player2 { get; protected set; } = null;
        #endregion

        #region Abstract methods
        public abstract void Register(Client client);
        public abstract void Search();
        #endregion

        #region Events
        protected void GameStateUpdate(object sender, OthelloTCPClientArgs e)
        {
            IsReady = true;
            GameState = e.GameState;
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
            if (IsReady)
                GetCurrentPlayer()?.Send(new PlayMoveOrder(coords));
        }
        #endregion
    }
}
