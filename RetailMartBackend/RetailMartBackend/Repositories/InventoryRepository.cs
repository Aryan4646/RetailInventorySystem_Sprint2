using RetailMartBackend.Models;
using RetailMartBackend.Interfaces;
using Microsoft.Data.SqlClient;

namespace RetailMartBackend.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly string _connectionString;

        public InventoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Inventory> GetInventory()
        {
            List<Inventory> inventoryList = new List<Inventory>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Inventory";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Inventory inventory = new Inventory
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        LastUpdated = Convert.ToDateTime(reader["LastUpdated"])
                    };

                    inventoryList.Add(inventory);
                }
            }

            return inventoryList;
        }

        public int GetAvailableStock(int productID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Quantity FROM Inventory WHERE ProductID = @ProductID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productID);

                connection.Open();
                object result = command.ExecuteScalar();

                if (result == null)
                {
                    return 0;
                }

                return Convert.ToInt32(result);
            }
        }

        public void AddStock(int productID, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Inventory
                                 SET Quantity = Quantity + @Quantity,
                                     LastUpdated = GETDATE()
                                 WHERE ProductID = @ProductID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@ProductID", productID);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void ReduceStock(int productID, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Inventory
                                 SET Quantity = Quantity - @Quantity,
                                     LastUpdated = GETDATE()
                                 WHERE ProductID = @ProductID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@ProductID", productID);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}