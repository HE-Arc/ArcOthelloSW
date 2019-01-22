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

        private Client client;
        private IOrderHandler orderHandler;

        public OthelloPlayerServer(TcpClient tcpClient)
        {
            //TODO Think about receiving a client or a TcpClient
            client = new Client();

            client.Bind(tcpClient);

            client.SetOrderhandler(this);
        }

        public void SetOrderhandler(IOrderHandler orderHandler)
        {
            this.orderHandler = orderHandler;
        }

        public void OpponentFound(string opponentName)
        {
            client.Send(new OpponentFoundOrder(opponentName));
        }

        public void OpponentAvatarChanged(int avatarId)
        {
            client.Send(new OpponentAvatarChangedOrder(avatarId));
        }

        public void HandleOrder(Order order)
        {
            switch (order)
            {
                #region Forwarded orders
                case AvatarChangedOrder castedOrder:
                    orderHandler?.HandleOrder(order);
                    break;

                

                #endregion
            }
        }
    }
}
