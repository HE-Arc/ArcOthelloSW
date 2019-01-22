using System;
using System.Net.Sockets;

namespace Tools
{
    public class OthelloPlayerServer : IOrderHandler
    {
        /// <summary>
        /// Get : get the Name binded
        /// Set : set the Name and inform the server of the change
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get : get the PlayerType
        /// Set : set the PlayerType
        /// </summary>
        public PlayerType PlayerType { get; private set; }

        /// <summary>
        /// Get : get the Color
        /// Set : set the Color
        /// </summary>
        public Color Color { get; private set; }

        private OthelloTCPClient client;
        private IOrderHandler orderHandler;

        public OthelloPlayerServer(TcpClient tcpClient)
        {
            //TODO Think about receiving a client or a TcpClient
            client = new OthelloTCPClient();

            client.Bind(tcpClient);

            client.SetOrderhandler(this);
        }

        public void SetOrderhandler(IOrderHandler orderHandler)
        {
            this.orderHandler = orderHandler;
        }

        /// <summary>
        /// Send the color the the client-side
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            client.Send(new AssignColorOrder(color));
        }

        /// <summary>
        /// Send the opponentName to the client-side
        /// </summary>
        /// <param name="opponentName"></param>
        public void OpponentFound(string opponentName)
        {
            client.Send(new OpponentFoundOrder(opponentName));
        }

        /// <summary>
        /// Send the opponent's avatarID to the client-side
        /// </summary>
        /// <param name="avatarId"></param>
        public void OpponentAvatarChanged(int avatarId)
        {
            client.Send(new OpponentAvatarChangedOrder(avatarId));
        }

        /// <summary>
        /// Handle received orders
        /// </summary>
        /// <param name="order"></param>
        public void HandleOrder(IOrderHandler sender, Order order)
        {
            // If null, sender is this object otherwise the order has been redirected
            sender = sender ?? this;

            switch (order)
            {
                #region Forwarded orders

                #region To GameHandler
                case AvatarChangedOrder a:
                case PlayerReadyOrder b:
                case PlayMoveOrder c:
                case UndoOrder d:
                case RedoOrder e:
                case SaveOrder f:
                    orderHandler?.HandleOrder(sender, order);
                    break;

                #endregion

                #region To TCPServer
                case RegisterRequestOrder castedOrder:
                    Name = !string.IsNullOrEmpty(castedOrder.Name) ? castedOrder.Name : throw new ArgumentException("name can't be null or empty");
                    PlayerType = (PlayerType)castedOrder.PlayerType;
                    orderHandler?.HandleOrder(sender, order);
                    break;
                #endregion

                #region To Matchmaker
                case SearchRequestOrder castedOrder:
                    orderHandler?.HandleOrder(sender, order);
                    break;

                case LoadOrder castedOrder:
                    orderHandler?.HandleOrder(sender, order);
                    break;
                    #endregion

                    #endregion
            }
        }
    }
}
