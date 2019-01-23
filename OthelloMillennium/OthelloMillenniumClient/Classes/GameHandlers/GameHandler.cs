using System;
using System.Threading;
using Tools;

namespace OthelloMillenniumClient.Classes.GameHandlers
{
    public abstract class GameHandler : IOrderHandler
    {
        public GameType GameType { get; protected set; }
        protected IOrderHandler orderHandler;

        protected Mutex stateMutex;

        public GameHandler()
        {
            stateMutex = new Mutex();
        }

        public void SetOrderHandler(IOrderHandler orderHandler)
        {
            this.orderHandler = orderHandler;
        }

        public abstract void LaunchGame();
        
        public abstract void HandleOrder(IOrderHandler sender, Order order);

        public abstract void AvatarIdChange(Color color, int avatarId);

        public abstract PlayerDataExport GetPlayers();

        public abstract Tuple<Color, Color> PlayersColor();
        public abstract int PlayersAvatarId(Color color);
        public abstract void Play(Tuple<char, int> columnRow);

        public abstract void Undo();
        public abstract void Redo();
    }
}
