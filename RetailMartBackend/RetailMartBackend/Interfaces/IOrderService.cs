using RetailMartBackend.Models;

namespace RetailMartBackend.Interfaces
{
    public interface IOrderService
    {
        Order CreateOrder(Order order);
        Order CreateOrderWithItems(Order order);
        Order GetOrderByID(int orderID);
        List<Order> GetOrdersByCustomerID(int customerID);
        Order UpdateOrderStatus(int orderID, string newStatus);
        decimal GetTotalSales();
    }
}
