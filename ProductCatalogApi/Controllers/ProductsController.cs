using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Commands.Products;
using ProductCatalogApi.Models;
using ProductCatalogApi.Queries.Products;
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
        private readonly IMediator _mediator;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger, IWebHostEnvironment env, IMediator mediator)
        {
            _productService = productService;
            _logger = logger;
            _env = env;
            _mediator = mediator;
        }

        // GET api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _mediator.Send(new GetAllProductsQuery());
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
                var product = await _mediator.Send(new GetProductByIdQuery(id));
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

        // Updated AddProductAsync method to handle potential null reference for Description property
        [HttpPost]
        public async Task<ActionResult<Product>> AddProductAsync(Product product) // Updated to async
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Send command to create the product
                var addedProduct = await _mediator.Send(new AddProductCommand
                {
                    Name = product.Name,
                    Description = product.Description ?? string.Empty, // Ensure non-null value
                    Price = product.Price
                });

                return CreatedAtAction(nameof(GetProductByIdAsync), new { id = addedProduct.Id }, addedProduct);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductsController.AddProductAsync(). Error: {ex.Message} Stacktrace: {ex.StackTrace}", product);

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the product.");
            }
        }
    }
}
