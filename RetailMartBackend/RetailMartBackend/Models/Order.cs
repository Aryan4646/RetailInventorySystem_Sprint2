namespace RetailMartBackend.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string? LastStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
    }
}
