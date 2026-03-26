using RetailMartBackend.Models;

namespace RetailMartBackend.Interfaces
{
    public interface IInventoryRepository
    {
        List<Inventory> GetInventory();
        int GetAvailableStock(int productID);
        void AddStock(int productID, int quantity);
        void ReduceStock(int productID, int quantity);
    }
}