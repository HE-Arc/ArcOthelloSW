using System;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace OthelloMillenniumServer
{
    /*
     * Parse orders and send them to gamemanager
     * keep clients informed
     */
    public class GameHandler : IOrderHandler
    {
        // Clients
        /// <summary>
        /// Socket server-side linked to a client remote
        /// </summary>
        public OthelloPlayerServer OthelloPlayer1 { get; private set; }

        /// <summary>
        /// Socket server-side linked to a client remote
        /// </summary>
        public OthelloPlayerServer OthelloPlayer2 { get; private set; }

        // GameManager
        public GameManager GameManager { get; private set; }

        public BattleType BattleType { get; private set; }

        /// <summary>
        /// Type of game currently being played
        /// </summary>
        public BattleType GameTypeData { get; private set; }

        private readonly object locker = new object();
        private bool client1Ready = false;
        private bool client2Ready = false;
        private readonly Task pinger;

        public GameHandler(OthelloPlayerServer black, OthelloPlayerServer white)
        {
            // Init Client 1
            OthelloPlayer1 = black;
            
            // Init Client 2
            OthelloPlayer2 = white;

            // TODO SEGAN (NiceToHave) : Ping clients to detect disconnect

            // Update clients
            OthelloPlayer1.SetColor(Color.Black);
            OthelloPlayer1.SetAvatarID(0);

            OthelloPlayer2.SetColor(Color.White);
            OthelloPlayer2.SetAvatarID(19);

            if (OthelloPlayer1.PlayerType == PlayerType.Human & OthelloPlayer2.PlayerType == PlayerType.Human)
                BattleType = BattleType.AgainstPlayer;
            else
                BattleType = BattleType.AgainstAI;

            // Init gameManager. TODO BASTIEN : Server don't know if it's local or online
            GameManager = new GameManager(GameType.Local);
            GameManager.OnGameFinished += GameManager_OnGameFinished;

            // Game is ready
            OthelloPlayer1.GameReady();
            OthelloPlayer2.GameReady();

            // Start to ping clients
            pinger.Start();
        }

        private OthelloPlayerServer GetOpponent(OthelloPlayerServer othelloPlayer)
        {
            return othelloPlayer.Equals(OthelloPlayer1) ? OthelloPlayer2 : OthelloPlayer1;
        }

        private void GameManager_OnGameFinished(object sender, GameState e)
        {
            var gameManager = sender as GameManager;
            var finalGameState = gameManager.Export();

            // Notifiy the end of the game
            OthelloPlayer1.GameEnded();
            OthelloPlayer2.GameEnded();

            // Send the final state
            BroadcastGameboard(); 
        }

        /// <summary>
        /// Send current gameState
        /// </summary>
        private void BroadcastGameboard()
        {
            var gs = GameManager.Export();
            OthelloPlayer1.UpdateGameboard(gs);
            OthelloPlayer2.UpdateGameboard(gs);
        }

        /// <summary>
        /// Start the game
        /// </summary>
        private void StartGame()
        {
            // Start the game
            GameManager.Start();

            // Informs the players that the game is starting
            OthelloPlayer1.GameStarted();
            OthelloPlayer2.GameStarted();

            // Send gameboard
            BroadcastGameboard();
        }

        public void SetOrderHandler(IOrderHandler handler)
        {
            throw new Exception("This object can not receive an handler");
        }

        public void HandleOrder(IOrderHandler sender, Order order)
        {
            // If null, sender is this object otherwise the order has been redirected
            sender = sender ?? this;

            switch (order)
            {
                case PlayMoveOrder castedOrder:
                    // Place a token on the board
                    GameManager.PlayMove(castedOrder.Coords, (sender as OthelloPlayerServer).Color);

                    // Send gameBoard to clients
                    BroadcastGameboard();
                    break;

                case GameStateRequestOrder castedOrder:
                    // A client asked for the gameState, send it back to him
                    (sender as OthelloPlayerServer).UpdateGameboard(GameManager.Export());
                    break;

                case AvatarChangedOrder castedOrder:
                    OthelloPlayerServer opponent = GetOpponent((sender as OthelloPlayerServer));
                    opponent.OpponentAvatarChanged(castedOrder.AvatarID);
                    break;

                case SaveRequestOrder castedOrder:
                    // Send the saved game
                    (sender as OthelloPlayerServer).TransferSaveOrder(GameManager.Save());
                    break;

                case UndoRequestOrder castedOrder:
                    try
                    {
                        // Goes one step back
                        GameManager.MoveBack();

                        // Goes back one more if the battle is against an IA
                        if (BattleType == BattleType.AgainstAI)
                            GameManager.MoveBack();

                        // Send gameboard
                        BroadcastGameboard();
                    }
                    catch (Exception ex)
                    {
                        Toolbox.LogError(ex);
                    }
                    break;

                case RedoRequestOrder castedOrder:
                    try
                    {
                        // Goes one step forward
                        GameManager.MoveForward();

                        // Goes forward one more if the battle is against an IA
                        if (BattleType == BattleType.AgainstAI)
                            GameManager.MoveForward();

                        // Send gameboard
                        BroadcastGameboard();
                    }
                    catch (Exception ex)
                    {
                        Toolbox.LogError(ex);
                    }
                    break;

                case PlayerReadyOrder castedOrder:
                    lock (locker)
                    {
                        var castedSender = (sender as OthelloPlayerServer);

                        if (castedSender.Color == Color.Black)
                            client1Ready = true;
                        else if (castedSender.Color == Color.White)
                            client2Ready = true;

                        if (client1Ready && client2Ready)
                            StartGame();
                    }
                    break;

                case Order unknownOrder:
                    throw new Exception("Unknown order received !");
            }
        }
    }
}
