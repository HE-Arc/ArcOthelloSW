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
        private bool load = false;
        private bool duplicatedGameEnded;
        
        public LocalGameHandler():base()
        {
            GameType = GameType.Local;
            readyToNextState = 0;

            // Start a local server
            TCPServer.Instance.StartListening(GameType.Local);
        }

        public void LoadGame(OthelloPlayerClient player1, OthelloPlayerClient player2)
        {
            load = true;
            JoinGame(player1, player2);
        }

        public void JoinGame(OthelloPlayerClient player1, OthelloPlayerClient player2)
        {
            this.player1 = player1;
            this.player2 = player2;
            duplicatedGameEnded = false;

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
            if(player1.PlayerState == PlayerState.MY_TURN)
            {
                player1.Undo();
            }
            else
            {
                player2.Undo();
            }
        }

        public override void Redo()
        {
            if (player1.PlayerState == PlayerState.MY_TURN)
            {
                player1.Redo();
            }
            else
            {
                player2.Redo();
            }
        }

        public override void HandleOrder(IOrderHandler sender, Order handledOrder)
        {
            lock (stateMutex)
            {
                switch (handledOrder)
                {
                    case RegisterSuccessfulOrder order:
                        if (!load)
                        {
                            if (player1.PlayerState == player2.PlayerState && player2.PlayerState == PlayerState.REGISTERED)
                            {
                                player1.SearchOpponent(player2.PlayerType);
                                player2.SearchOpponent(player1.PlayerType);
                            }
                        }
                        else
                        {
                            player1.Load();
                        }
                        orderHandler.HandleOrder(sender, order);
                        break;

                    case OpponentFoundOrder order:
                        if (player1.PlayerState == player2.PlayerState && player1.PlayerState == PlayerState.BINDED)
                        {
                            orderHandler.HandleOrder(sender, order);
                        }
                        break;

                    case GameReadyOrder order:
                        readyToNextState++;
                        if (player1.PlayerState == player2.PlayerState && player1.PlayerState == PlayerState.LOBBY_CHOICE && readyToNextState == 2)
                        {
                            readyToNextState = 0;
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
                        if (duplicatedGameEnded)
                        {
                            orderHandler.HandleOrder(sender, order);
                        }
                        duplicatedGameEnded = !duplicatedGameEnded;
                        break;

                    case SaveResponseOrder order:
                        orderHandler.HandleOrder(sender, order);
                        break;

                    case LoadResponseOrder order:
                        if(load && player2.PlayerState == PlayerState.REGISTERED)
                        {
                            player2.JoinGame(order.GameID);
                        }
                        orderHandler.HandleOrder(sender, order);
                        break;

                    case OpponentAvatarChangedOrder order:
                        // Nothing -> no need to update imagePlayer in local game
                        break;
                }
            }
            
        }

        /// <summary>
        /// Player1 perfom the save
        /// </summary>
        public override void Save()
        {
            player1.Save();
        }
    }
}
