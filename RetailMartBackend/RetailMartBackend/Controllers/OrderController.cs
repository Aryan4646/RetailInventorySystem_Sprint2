using Microsoft.AspNetCore.Mvc;
using RetailMartBackend.Interfaces;
using RetailMartBackend.Models;
using RetailMartBackend.Exceptions;

namespace RetailMartBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            try
            {
                var result = _orderService.CreateOrder(order);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (OutOfStockException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("with-items")]
        public IActionResult CreateOrderWithItems(Order order)
        {
            try
            {
                var result = _orderService.CreateOrderWithItems(order);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (OutOfStockException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderByID(int id)
        {
            try
            {
                var result = _orderService.GetOrderByID(id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("customer/{customerID}")]
        public IActionResult GetOrdersByCustomerID(int customerID)
        {
            try
            {
                var result = _orderService.GetOrdersByCustomerID(customerID);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateOrderStatus(int id, [FromBody] string newStatus)
        {
            try
            {
                var result = _orderService.UpdateOrderStatus(id, newStatus);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOrderStatusException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("totalsales")]
        public IActionResult GetTotalSales()
        {
            try
            {
                var total = _orderService.GetTotalSales();
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
