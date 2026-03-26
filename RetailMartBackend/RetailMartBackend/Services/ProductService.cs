using RetailMartBackend.Models;
using RetailMartBackend.Interfaces;
using RetailMartBackend.Exceptions;

namespace RetailMartBackend.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public List<Product> GetProducts()
        {
            return _productRepository.GetProducts();
        }

        public void AddProduct(Product product)
        {
            _productRepository.AddProduct(product);
        }

        public void UpdateProduct(Product product)
        {
            _productRepository.UpdateProduct(product);
        }

        public Product GetProductByID(int productID)
        {
            var product = _productRepository.GetProductByID(productID);

            if (product == null)
            {
                throw new NotFoundException("Product not found.");
            }

            return product;
        }
    }
}