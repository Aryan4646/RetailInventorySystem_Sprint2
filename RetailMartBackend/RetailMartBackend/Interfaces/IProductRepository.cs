using RetailMartBackend.Models;

namespace RetailMartBackend.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetProducts();
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        Product GetProductByID(int productID);
    }
}
