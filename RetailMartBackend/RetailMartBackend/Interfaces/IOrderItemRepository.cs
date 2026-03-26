using RetailMartBackend.Models;

namespace RetailMartBackend.Interfaces
{
    public interface IOrderItemRepository
    {
        void AddOrderItem(OrderItem orderItem);
        List<OrderItem> GetOrderItemsByOrderID(int orderID);
    }
}
