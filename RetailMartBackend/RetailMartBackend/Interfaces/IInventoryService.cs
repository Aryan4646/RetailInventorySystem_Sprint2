using RetailMartBackend.Models;
namespace RetailMartBackend.Interfaces
{
    public interface IInventoryService
    {
        List<Inventory> GetInventory();
        void UpdateStock(int ProductID, int Quantity);
    }
}
