using RetailMartBackend.Models;
using RetailMartBackend.Interfaces;
using RetailMartBackend.Exceptions;

namespace RetailMartBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IInventoryRepository inventoryRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
        }

        public Order CreateOrder(Order order)
        {
            if (!_customerRepository.CustomerExists(order.CustomerID))
            {
                throw new NotFoundException("Customer not found.");
            }

            if (order.Quantity <= 0)
            {
                throw new Exception("Quantity must be greater than 0.");
            }

            var product = _productRepository.GetProductByID(order.ProductID);

            if (product == null)
            {
                throw new NotFoundException("Product not found.");
            }

            int availableStock = _inventoryRepository.GetAvailableStock(order.ProductID);

            if (availableStock < order.Quantity)
            {
                throw new OutOfStockException($"Only {availableStock} item(s) available in stock.");
            }

            order.TotalAmount = product.Price * order.Quantity;

            if (order.OrderDate == default)
            {
                order.OrderDate = DateTime.Now;
            }

            if (string.IsNullOrEmpty(order.LastStatus))
            {
                order.LastStatus = "Created";
            }

            Order createdOrder = _orderRepository.CreateOrder(order);

            _inventoryRepository.ReduceStock(order.ProductID, order.Quantity);

            return createdOrder;
        }

        public Order CreateOrderWithItems(Order order)
        {
            if (!_customerRepository.CustomerExists(order.CustomerID))
            {
                throw new NotFoundException("Customer not found.");
            }

            if (order.OrderItems == null || order.OrderItems.Count == 0)
            {
                throw new Exception("Order must have at least one item.");
            }

            if (order.OrderDate == default)
            {
                order.OrderDate = DateTime.Now;
            }

            if (string.IsNullOrEmpty(order.LastStatus))
            {
                order.LastStatus = "Created";
            }

            decimal grandTotal = 0;

            foreach (var item in order.OrderItems)
            {
                if (item.Quantity <= 0)
                {
                    throw new Exception($"Quantity for ProductID {item.ProductID} must be greater than 0.");
                }

                var product = _productRepository.GetProductByID(item.ProductID);

                if (product == null)
                {
                    throw new NotFoundException($"Product with ID {item.ProductID} not found.");
                }

                int availableStock = _inventoryRepository.GetAvailableStock(item.ProductID);

                if (availableStock < item.Quantity)
                {
                    throw new OutOfStockException($"Product ID {item.ProductID} '{product.ProductName}': only {availableStock} item(s) in stock.");
                }

                item.UnitPrice = product.Price;
                item.LineTotal = product.Price * item.Quantity;
                grandTotal += item.LineTotal;
            }

            var firstItem = order.OrderItems[0];
            order.ProductID = firstItem.ProductID;
            order.Quantity = firstItem.Quantity;
            order.TotalAmount = grandTotal;

            Order createdOrder = _orderRepository.CreateOrder(order);

            foreach (var item in order.OrderItems)
            {
                item.OrderID = createdOrder.OrderID;
                _orderItemRepository.AddOrderItem(item);
                _inventoryRepository.ReduceStock(item.ProductID, item.Quantity);
            }

            createdOrder.OrderItems = order.OrderItems;

            return createdOrder;
        }

        public Order GetOrderByID(int orderID)
        {
            var order = _orderRepository.GetOrderByID(orderID);

            if (order == null)
            {
                throw new NotFoundException("Order not found.");
            }

            order.OrderItems = _orderItemRepository.GetOrderItemsByOrderID(orderID);

            return order;
        }

        public List<Order> GetOrdersByCustomerID(int customerID)
        {
            if (!_customerRepository.CustomerExists(customerID))
            {
                throw new NotFoundException("Customer not found.");
            }

            var orders = _orderRepository.GetOrdersByCustomerID(customerID);

            foreach (var order in orders)
            {
                order.OrderItems = _orderItemRepository.GetOrderItemsByOrderID(order.OrderID);
            }

            return orders;
        }

        public Order UpdateOrderStatus(int orderID, string newStatus)
        {
            var order = _orderRepository.GetOrderByID(orderID);

            if (order == null)
            {
                throw new NotFoundException("Order not found.");
            }

            var allowedTransitions = new Dictionary<string, List<string>>
            {
                { "Created",    new List<string> { "Processing", "Cancelled" } },
                { "Processing", new List<string> { "Shipped",    "Cancelled" } },
                { "Shipped",    new List<string> { "Delivered",  "Cancelled" } },
                { "Delivered",  new List<string>() },
                { "Cancelled",  new List<string>() }
            };

            if (!allowedTransitions.ContainsKey(order.LastStatus))
            {
                throw new InvalidOrderStatusException($"Current status '{order.LastStatus}' is not recognized.");
            }

            if (!allowedTransitions[order.LastStatus].Contains(newStatus))
            {
                throw new InvalidOrderStatusException($"Cannot move order from '{order.LastStatus}' to '{newStatus}'.");
            }

            return _orderRepository.UpdateOrderStatus(orderID, newStatus);
        }

        public decimal GetTotalSales()
        {
            return _orderRepository.GetTotalSales();
        }
    }
}