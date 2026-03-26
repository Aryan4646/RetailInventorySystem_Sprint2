using RetailMartBackend.Interfaces;
using RetailMartBackend.Models;
using RetailMartBackend.Exceptions;

namespace RetailMartBackend.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public List<Inventory> GetInventory()
        {
            return _inventoryRepository.GetInventory();
        }

        public void UpdateStock(int ProductID, int Quantity)
        {
            int availableStock = _inventoryRepository.GetAvailableStock(ProductID);

            if (availableStock == 0)
                throw new NotFoundException("Product not found or no inventory record exists.");

            if (Quantity < 0)
            {
                if (availableStock < Math.Abs(Quantity))
                    throw new OutOfStockException("Not enough stock.");

                _inventoryRepository.ReduceStock(ProductID, Math.Abs(Quantity));
            }
            else if (Quantity > 0)
            {
                _inventoryRepository.AddStock(ProductID, Quantity);
            }
        }
    }
}