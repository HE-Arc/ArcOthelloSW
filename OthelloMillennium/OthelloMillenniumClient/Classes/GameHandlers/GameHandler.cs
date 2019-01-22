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

        public void SetOrderHandler(IOrderHandler orderHandler)
        {
            this.orderHandler = orderHandler;
        }

        public abstract void LaunchGame();
        
        public abstract void HandleOrder(IOrderHandler sender, Order order);
    }
}
