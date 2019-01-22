namespace Tools
{
    public interface IOrderHandler
    {
        void HandleOrder(IOrderHandler sender, Order order);
    }
}
