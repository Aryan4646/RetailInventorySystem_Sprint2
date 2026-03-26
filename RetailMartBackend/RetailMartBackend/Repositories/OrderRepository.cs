using RetailMartBackend.Interfaces;
using RetailMartBackend.Models;
using Microsoft.Data.SqlClient;

namespace RetailMartBackend.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Order CreateOrder(Order order)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Orders (CustomerID, ProductID, Quantity, OrderDate, LastStatus, TotalAmount)
                                 OUTPUT INSERTED.OrderID
                                 VALUES (@CustomerID, @ProductID, @Quantity, @OrderDate, @LastStatus, @TotalAmount)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerID", order.CustomerID);
                command.Parameters.AddWithValue("@ProductID", order.ProductID);
                command.Parameters.AddWithValue("@Quantity", order.Quantity);
                command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                command.Parameters.AddWithValue("@LastStatus", order.LastStatus);
                command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);

                connection.Open();
                order.OrderID = (int)command.ExecuteScalar();
            }

            return order;
        }

        public Order GetOrderByID(int orderID)
        {
            Order order = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT OrderID, CustomerID, ProductID, Quantity, OrderDate, LastStatus, TotalAmount
                                 FROM Orders
                                 WHERE OrderID = @OrderID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderID", orderID);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    order = new Order
                    {
                        OrderID = Convert.ToInt32(reader["OrderID"]),
                        CustomerID = Convert.ToInt32(reader["CustomerID"]),
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                        LastStatus = reader["LastStatus"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"])
                    };
                }
            }

            return order;
        }

        public List<Order> GetOrdersByCustomerID(int customerID)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT OrderID, CustomerID, ProductID, Quantity, OrderDate, LastStatus, TotalAmount
                                 FROM Orders
                                 WHERE CustomerID = @CustomerID
                                 ORDER BY OrderDate DESC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerID", customerID);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderID = Convert.ToInt32(reader["OrderID"]),
                        CustomerID = Convert.ToInt32(reader["CustomerID"]),
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                        LastStatus = reader["LastStatus"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"])
                    });
                }
            }

            return orders;
        }

        public Order UpdateOrderStatus(int orderID, string newStatus)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Orders SET LastStatus = @LastStatus WHERE OrderID = @OrderID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@LastStatus", newStatus);
                command.Parameters.AddWithValue("@OrderID", orderID);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return GetOrderByID(orderID);
        }

        public decimal GetTotalSales()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM Orders WHERE LastStatus = 'Delivered'";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                return Convert.ToDecimal(command.ExecuteScalar());
            }
        }
    }
}
