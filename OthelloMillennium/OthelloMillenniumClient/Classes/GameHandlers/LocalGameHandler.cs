using OthelloMillenniumServer;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace OthelloMillenniumClient.Classes.GameHandlers
{
    public class LocalGameHandler : GameHandler, IOrderHandler
    {

        private OthelloPlayerClient player1;
        private OthelloPlayerClient player2;

        private int readyToNextState;
        
        public LocalGameHandler():base()
        {
            GameType = GameType.Local;
            readyToNextState = 0;

            // Test if server has been started
            if (StartLocalServer() < 0)
            {
                throw new Exception($"Unable to start server on port {TCPServer.Instance.Port}");
            }
        }

        public void JoinGame(OthelloPlayerClient player1, OthelloPlayerClient player2)
        {
            this.player1 = player1;
            this.player2 = player2;

            player1.SetOrderHandler(this);
            player2.SetOrderHandler(this);

            player1.Connect(GameType);
            player2.Connect(GameType);

            Thread.Sleep(1000);

            player1.Register();
            player2.Register();
        }

        public override void LaunchGame()
        {
            player1.ReadyToPlay();
            player2.ReadyToPlay();
        }

        public override Tuple<Color, Color> PlayersColor() => new Tuple<Color, Color>(player1.Color, player2.Color);

        public override PlayerDataExport GetPlayers() => new PlayerDataExport()
            {
                Name1 = player1.Name,
                Name2 = player2.Name,

                Color1 = player1.Color,
                Color2 = player2.Color,

                AvatarId1 = player1.AvatarId,
                AvatarId2 = player2.AvatarId
            };

        public override int PlayersAvatarId(Color color) => player1.Color == color ? player1.AvatarId : player2.AvatarId;

        public override void Play(Tuple<char, int> columnRow)
        {
            if(player1.PlayerState == PlayerState.MY_TURN)
            {
                player1.Play(columnRow.Item1, columnRow.Item2);
            }
            else
            {
                player2.Play(columnRow.Item1, columnRow.Item2);
            }
        }

        public override void AvatarIdChange(Color color, int avatarId)
        {
            if(player1.Color == color)
            {
                player1.AvatarId = avatarId;
            }
            else
            {
                player2.AvatarId = avatarId;
            }
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }

        public override void Redo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Start a local server
        /// </summary>
        /// <returns>Port where server is listening or -1 if it failed</returns>
        private int StartLocalServer()
        {
            try
            {
                int port = (new Random()).Next(49152, 65535);
                TCPServer.Instance.Port = port;
                TCPServer.Instance.StartListening();
                return port;
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);
                return -1;
            }
        }

        public override void HandleOrder(IOrderHandler sender, Order handledOrder)
        {
            lock (stateMutex)
            {
                switch (handledOrder)
                {
                    case RegisterSuccessfulOrder order:
                        if (player1.PlayerState == player2.PlayerState && player2.PlayerState == PlayerState.REGISTERED)
                        {
                            player1.SearchOpponent(player2.PlayerType);
                            player2.SearchOpponent(player1.PlayerType);
                            orderHandler.HandleOrder(sender, order);
                        }
                        break;

                    case OpponentFoundOrder order:
                        if (player1.PlayerState == player2.PlayerState && player1.PlayerState == PlayerState.BINDED)
                        {
                            orderHandler.HandleOrder(sender, order);
                        }
                        break;

                    case GameReadyOrder order:
                        if (player1.PlayerState == player2.PlayerState && player1.PlayerState == PlayerState.LOBBY_CHOICE && readyToNextState == 2)
                        {
                            orderHandler.HandleOrder(sender, order);
                        }
                        break;

                    case GameStartedOrder order:
                        if (player1.PlayerState == PlayerState.MY_TURN && player2.PlayerState == PlayerState.OPPONENT_TURN ||
                            player1.PlayerState == PlayerState.OPPONENT_TURN && player2.PlayerState == PlayerState.MY_TURN)
                        {
                            orderHandler.HandleOrder(sender, order);
                        }
                        break;
                        
                    case UpdateGameStateOrder order:
                        orderHandler.HandleOrder(sender, order);
                        break;

                    case GameEndedOrder order:
                        orderHandler.HandleOrder(sender, order);
                        break;

                    case OpponentAvatarChangedOrder order:
                        // Nothing -> no need to update imagePlayer in local game
                        break;
                }
            }
            
        }
    }
}
