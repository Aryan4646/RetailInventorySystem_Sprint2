namespace RetailMartBackend.Interfaces
{
    public interface ICustomerRepository
    {
        bool CustomerExists(int customerID);
    }
}