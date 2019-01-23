using System;
using System.Threading;
using Tools;

namespace OthelloMillenniumClient.Classes.GameHandlers
{
    public class OnlineGameHandler : GameHandler
    {
        private OthelloPlayerClient player1;
        private RemotePlayerData player2;
        
        public OnlineGameHandler() : base()
        {
            GameType = GameType.Online;
        }

        public void JoinGame(OthelloPlayerClient player, BattleType battleType)
        {
            player1 = player;
            player2 = new RemotePlayerData()
            {
                Name = "[DEFAULT]",
                AvatarId = 15,
                PlayerType = battleType == BattleType.AgainstPlayer ? PlayerType.Human : PlayerType.AI
            };

            player1.SetOrderHandler(this);

            player1.Connect(GameType);
            Thread.Sleep(1000);
            player1.Register();
        }

        public override void LaunchGame()
        {
            player1.ReadyToPlay();
        }

        public override Tuple<Color, Color> PlayersColor() => new Tuple<Color, Color>(player1.Color, player1.Color==Color.Black?Color.White:Color.Black);

        public override PlayerDataExport GetPlayers() => new PlayerDataExport()
        {
            Name1 = player1.Name,
            Name2 = player2.Name,

            Color1 = player1.Color,
            Color2 = player1.Color == Color.Black ? Color.Black : Color.White,

            AvatarId1 = player1.AvatarId,
            AvatarId2 = player2.AvatarId
        };

        public override int PlayersAvatarId(Color color) => player1.Color == color ? player1.AvatarId : player2.AvatarId;

        public override void Play(Tuple<char, int> columnRow)
        {
            if (player1.PlayerState == PlayerState.MY_TURN)
            {
                player1.Play(columnRow.Item1, columnRow.Item2);
            }
            else
            {
                return;
            }
        }

        public override void Undo()
        {
            throw new Exception("Undo/Redo order not allowed during online games");
        }

        public override void Redo()
        {
            throw new Exception("Undo/Redo order not allowed during online games");
        }

        public override void HandleOrder(IOrderHandler sender, Order handledOrder)
        {
            lock (stateMutex)
            {
                switch (handledOrder)
                {
                    case RegisterSuccessfulOrder order:
                        player1.SearchOpponent(player2.PlayerType);
                        orderHandler.HandleOrder(sender, order);
                        break;

                    case OpponentFoundOrder order:
                        OpponentFoundOrder opponentFoundOrder = order;
                        player2.Name = opponentFoundOrder.OpponentName;
                        orderHandler.HandleOrder(sender, order);
                        break;

                    case GameReadyOrder order:
                        orderHandler.HandleOrder(sender, order);
                        break;

                    case GameStartedOrder order:
                        orderHandler.HandleOrder(sender, order);
                        break;

                    case OpponentAvatarChangedOrder order:
                        orderHandler.HandleOrder(sender, order);
                        break;

                    case UpdateGameStateOrder order:
                        orderHandler.HandleOrder(sender, order);
                        break;

                    case GameEndedOrder order:
                        orderHandler.HandleOrder(sender, order);
                        break;
                }
            }
        }

        public override void AvatarIdChange(Color color, int avatarId)
        {
            if(color != player1.Color)
            {
                throw new Exception("Try to change the avatar of the opponent");
            }
            player1.AvatarId = avatarId;
        }

        public override void Save()
        {
            player1.Save();
        }
    }
}
