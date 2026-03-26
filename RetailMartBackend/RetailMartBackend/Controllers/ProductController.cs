using Microsoft.AspNetCore.Mvc;
using RetailMartBackend.Interfaces;
using RetailMartBackend.Models;
using RetailMartBackend.Exceptions;

namespace RetailMartBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            try
            {
                return Ok(_productService.GetProducts());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetProductByID(int id)
        {
            try
            {
                var product = _productService.GetProductByID(id);
                return Ok(product);
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

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            try
            {
                _productService.AddProduct(product);
                return Ok("Product added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateProduct(Product product)
        {
            try
            {
                _productService.UpdateProduct(product);
                return Ok("Product updated successfully");
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
    }
}