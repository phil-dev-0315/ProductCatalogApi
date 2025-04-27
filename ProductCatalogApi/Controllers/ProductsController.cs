using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Models;
using ProductCatalogApi.Services;

namespace ProductCatalogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {

        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;
        private readonly IWebHostEnvironment _env;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger, IWebHostEnvironment env)
        {
            _productService = productService;
            _logger = logger;
            _env = env;
        }

        // GET api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductsController.GetAllProductsAsync(). Error: {ex.Message} Stacktrace: {ex.StackTrace}");
                var message = _env.IsDevelopment() ? ex.Message : "An error occurred while retrieving products.";
                return StatusCode(StatusCodes.Status500InternalServerError, message);
            }
        }

        // GET api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductByIdAsync(int id) // This is where we get by id
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { Message = $"Product with ID {id} not found." });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductsController.GetProductByIdAsync({id}). Error: {ex.Message} Stacktrace: {ex.StackTrace}");
                var message = _env.IsDevelopment() ? ex.Message : "An error occurred while retrieving the product.";
                return StatusCode(StatusCodes.Status500InternalServerError, message);
            }
        }

        // POST api/products
        [HttpPost]
        public async Task<ActionResult<Product>> AddProductAsync(Product product) // Updated to async
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _productService.AddProductAsync(product);

                if (product.Id <= 0)
                {
                    return BadRequest(new { Message = "Product ID was not generated." });
                }

                return CreatedAtAction(nameof(GetProductByIdAsync), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductsController.AddProductAsync(). Error: {ex.Message} Stacktrace: {ex.StackTrace}",product);

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the product.");
            }
        }
    }
}
