using Tools;
using Tools.Classes;

namespace OthelloMillenniumClient.Classes
{
    public abstract class GameHandler
    {
        #region Properties
        public GameType GameType { get; } = GameType.Local;
        public BattleType BattleType { get; private set; }

        public Client Client { get; protected set; }
        public Client Opponent { get; protected set; }

        public bool IsGameReady { get; private set; }
        public GameState GameState { get; private set; }
        #endregion

        #region Attributes
        protected bool isClientTurn;
        #endregion

        protected GameHandler(BattleType battleType)
        {
            BattleType = battleType;
        }

        public Client GetCurrentPlayer()
        {
            return isClientTurn ? Client : Opponent;
        }

        public abstract void Init();

        protected void Client_OnBeginReceived(object sender, OthelloTCPClientArgs e)
        {
            isClientTurn = true;
        }

        protected void Client_OnAwaitReceived(object sender, OthelloTCPClientArgs e)
        {
            isClientTurn = false;
        }

        protected abstract void Client_OnOpponentFound(object sender, OthelloTCPClientArgs e);

        protected void Client_OnGameStartedReceived(object sender, OthelloTCPClientArgs e)
        {
            IsGameReady = true;
        }

        protected void Client_OnGameStateReceived(object sender, OthelloTCPClientArgs e)
        {
            GameState = e.GameState;
        }
    }
}
