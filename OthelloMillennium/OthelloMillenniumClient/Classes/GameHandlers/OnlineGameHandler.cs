using System;
using Tools;

namespace OthelloMillenniumClient.Classes.GameHandlers
{
    public class OnlineGameHandler : GameHandler
    {
        private OthelloPlayerClient player1;
        private RemotePlayerData player2;

        public OnlineGameHandler()
        {
            GameType = GameType.Online;
        }

        public void JoinGame(PlayerType playerOne, string playerNameOne, BattleType battleType)
        {
            player1 = new OthelloPlayerClient(playerOne, playerNameOne);
            player2 = new RemotePlayerData();

            player1.Connect(GameType);
            player1.Register();

            // TODO Wait for registration completed

            player1.SearchOpponent(battleType == BattleType.AgainstPlayer ? PlayerType.Human : PlayerType.AI);
        }

        public override void LaunchGame()
        {
            player1.ReadyToPlay();

            //TODO Goto Game when both player are ready
        }

        public override Tuple<Color, Color> PlayersColor() => new Tuple<Color, Color>(player1.Color, player1.Color==Color.Black?Color.White:Color.Black);
        public override Tuple<int, Color> PlayersAvatarId() => new Tuple<int, Color>(player1.AvatarID, player1.Color);

        public override void HandleOrder(IOrderHandler sender, Order handledOrder)
        {
            switch (handledOrder)
            {
                case RegisterSuccessfulOrder order:
                    //TODO
                    orderHandler?.HandleOrder(sender, order);
                    break;

                case OpponentFoundOrder order:
                    //TODO
                    orderHandler?.HandleOrder(sender, order);
                    break;

                case GameReadyOrder order:
                    //TODO
                    orderHandler?.HandleOrder(sender, order);
                    break;

                case GameStartedOrder order:
                    //TODO
                    orderHandler?.HandleOrder(sender, order);
                    break;

                case OpponentAvatarChangedOrder order:
                    //TODO
                    orderHandler?.HandleOrder(sender, order);
                    break;
            }
        }

        public override void AvatarIdChange(Color color, int avatarId)
        {
            if(color != player1.Color)
            {
                throw new Exception("Try to change the avatar of the opponent");
            }
            player1.AvatarID = avatarId;
        }
    }
}
