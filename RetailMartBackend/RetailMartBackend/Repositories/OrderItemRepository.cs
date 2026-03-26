using RetailMartBackend.Interfaces;
using RetailMartBackend.Models;
using Microsoft.Data.SqlClient;

namespace RetailMartBackend.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly string _connectionString;

        public OrderItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO OrderItems (OrderID, ProductID, Quantity, UnitPrice, LineTotal)
                                 VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice, @LineTotal)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderID", orderItem.OrderID);
                command.Parameters.AddWithValue("@ProductID", orderItem.ProductID);
                command.Parameters.AddWithValue("@Quantity", orderItem.Quantity);
                command.Parameters.AddWithValue("@UnitPrice", orderItem.UnitPrice);
                command.Parameters.AddWithValue("@LineTotal", orderItem.LineTotal);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<OrderItem> GetOrderItemsByOrderID(int orderID)
        {
            List<OrderItem> items = new List<OrderItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT OrderItemID, OrderID, ProductID, Quantity, UnitPrice, LineTotal
                                 FROM OrderItems
                                 WHERE OrderID = @OrderID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderID", orderID);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(new OrderItem
                    {
                        OrderItemID = Convert.ToInt32(reader["OrderItemID"]),
                        OrderID = Convert.ToInt32(reader["OrderID"]),
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                        LineTotal = Convert.ToDecimal(reader["LineTotal"])
                    });
                }
            }

            return items;
        }
    }
}
