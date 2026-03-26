using Microsoft.AspNetCore.Mvc;
using RetailMartBackend.Interfaces;
using RetailMartBackend.Models;
using RetailMartBackend.Exceptions;

namespace RetailMartBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public ActionResult<List<Inventory>> GetInventory()
        {
            return Ok(_inventoryService.GetInventory());
        }

        [HttpPut("{productID}/{quantity}")]
        public ActionResult UpdateStock(int productID, int quantity)
        {
            try
            {
                _inventoryService.UpdateStock(productID, quantity);
                return Ok("Stock updated successfully");
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
    }
}