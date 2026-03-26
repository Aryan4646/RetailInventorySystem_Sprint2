using RetailMartBackend.Models;
using RetailMartBackend.Interfaces;
using Microsoft.Data.SqlClient;

namespace RetailMartBackend.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Product> GetProducts()
        {
            List<Product> productList = new List<Product>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT ProductID, ProductName, Price, SupplierID FROM Products";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Product product = new Product
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        ProductName = reader["ProductName"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        SupplierID = Convert.ToInt32(reader["SupplierID"])
                    };

                    productList.Add(product);
                }
            }

            return productList;
        }

        public void AddProduct(Product product)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Products (ProductName, Price, SupplierID)
                                 VALUES (@ProductName, @Price, @SupplierID)";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ProductName", product.ProductName);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@SupplierID", product.SupplierID);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void UpdateProduct(Product product)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Products
                                 SET ProductName = @ProductName,
                                     Price = @Price,
                                     SupplierID = @SupplierID
                                 WHERE ProductID = @ProductID";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ProductID", product.ProductID);
                command.Parameters.AddWithValue("@ProductName", product.ProductName);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@SupplierID", product.SupplierID);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public Product GetProductByID(int productID)
        {
            Product product = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT ProductID, ProductName, Price, SupplierID
                                 FROM Products
                                 WHERE ProductID = @ProductID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productID);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    product = new Product
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        ProductName = reader["ProductName"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        SupplierID = Convert.ToInt32(reader["SupplierID"])
                    };
                }
            }

            return product;
        }
    }
}
