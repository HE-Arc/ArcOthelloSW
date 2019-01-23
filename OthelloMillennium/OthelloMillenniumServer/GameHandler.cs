using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Assign player and handle their orders
        /// <para/>Call Load(...) afterward
        /// </summary>
        /// <param name="black"></param>
        /// <param name="white"></param>
        internal GameHandler(LoadRequest loadRequest)
            : this(loadRequest.Player1, loadRequest.Player2)
        {
            Load(loadRequest.GameStates);
        }

        /// <summary>
        /// Assign player and handle their orders
        /// <para/>Call either Init or Load afterward
        /// </summary>
        /// <param name="black"></param>
        /// <param name="white"></param>
        public GameHandler(OthelloPlayerServer black, OthelloPlayerServer white)
        {
            // Init Client 1
            OthelloPlayer1 = black;
            
            // Init Client 2
            OthelloPlayer2 = white;

            // Update handler
            OthelloPlayer1.SetOrderHandler(this);
            OthelloPlayer2.SetOrderHandler(this);

            if (OthelloPlayer1.PlayerType == PlayerType.Human & OthelloPlayer2.PlayerType == PlayerType.Human)
                BattleType = BattleType.AgainstPlayer;
            else
                BattleType = BattleType.AgainstAI;

            // TODO SEGAN (NiceToHave) : Ping clients to detect disconnect
            // Start to ping clients
            // pinger.Start();
        }

        public void Init()
        {
            // Init gameManager
            GameManager = new GameManager();
            GameManager.OnGameFinished += GameManager_OnGameFinished;

            // Update clients
            OthelloPlayer1.SetColor(Color.Black);
            OthelloPlayer1.SetAvatarID(0);
            OthelloPlayer2.SetColor(Color.White);
            OthelloPlayer2.SetAvatarID(19);

            // Game is ready
            OthelloPlayer1.GameReady();
            OthelloPlayer2.GameReady();
        }

        /// <summary>
        /// Send avatardID from both client after this function
        /// </summary>
        /// <param name="game"></param>
        public void Load(List<GameState> game)
        {
            // Init gameManager
            GameManager = new GameManager();
            GameManager.OnGameFinished += GameManager_OnGameFinished;

            // Load the game
            GameManager.Load(game);

            // Game is ready
            OthelloPlayer1.GameReady();
            OthelloPlayer2.GameReady();

            // Immediatly start the game
            StartGame();
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

            // Get initial gameState
            var gs = GameManager.Export();

            // Informs the players that the game is starting
            OthelloPlayer1.GameStarted(gs);
            OthelloPlayer2.GameStarted(gs);
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
                    (sender as OthelloPlayerServer).SaveSuccessful(GameManager.Save());
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
                        Console.Error.WriteLine("Undo not available");
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
                        Console.Error.WriteLine("Redo not available");
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
