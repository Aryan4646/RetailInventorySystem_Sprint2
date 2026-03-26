namespace RetailMartBackend.Exceptions
{
    public class InvalidOrderStatusException : Exception
    {
        public InvalidOrderStatusException(string message) : base(message)
        {
        }
    }
}