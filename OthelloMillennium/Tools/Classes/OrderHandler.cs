namespace Tools
{
    public interface IOrderHandler
    {
        void SetOrderHandler(IOrderHandler handler);
        void HandleOrder(IOrderHandler sender, Order order);
    }
}
