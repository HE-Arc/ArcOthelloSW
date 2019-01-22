using System;
using Tools;

namespace OthelloMillenniumClient.Classes.GameHandlers
{
    public abstract class GameHandler : IOrderHandler
    {
        public GameType GameType { get; protected set; }
        protected IOrderHandler orderHandler;

        public GameHandler()
        {
            //Nothing
        }

        public void SetOrderhandler(IOrderHandler orderHandler)
        {
            this.orderHandler = orderHandler;
        }

        public abstract void LaunchGame();
        
        public abstract void HandleOrder(Order order);
    }
}
