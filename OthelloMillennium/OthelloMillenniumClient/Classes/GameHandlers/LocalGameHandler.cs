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

        public LocalGameHandler()
        {
            GameType = GameType.Local;

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

            player1.Connect(GameType);
            player2.Connect(GameType);

            player1.Register();
            player2.Register();

            // TODO Wait for registration completed

            player1.SearchOpponent(player2.PlayerType);
            player2.SearchOpponent(player1.PlayerType);
        }

        public override void LaunchGame()
        {
            player1.ReadyToPlay();
            player2.ReadyToPlay();

            //TODO Goto Game when both are ready
        }

        public override Tuple<Color, Color> PlayersColor() => new Tuple<Color, Color>(player1.Color, player2.Color);

        public override Tuple<int, Color> PlayersAvatarId() => new Tuple<int, Color>(player1.AvatarID, player1.Color);

        public override void AvatarIdChange(Color color, int avatarId)
        {
            if(player1.Color == color)
            {
                player1.AvatarID = avatarId;
            }
            else
            {
                player2.AvatarID = avatarId;
            }
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
                    // Nothing -> no need to update imagePlayer in local game
                    break;
            }
        }
    }
}
