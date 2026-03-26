using RetailMartBackend.Models;
namespace RetailMartBackend.Interfaces
{
    public interface IProductService
    {
        List<Product> GetProducts();
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        Product GetProductByID(int productID);
    }
}
