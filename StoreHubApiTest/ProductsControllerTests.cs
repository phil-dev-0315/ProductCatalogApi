using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using ProductCatalogApi.Controllers;
using ProductCatalogApi.Models;
using ProductCatalogApi.Services;

namespace ProductCatalogApiTest
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductsController _controller;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly Mock<IWebHostEnvironment> _mockEnv;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _mockEnv = new Mock<IWebHostEnvironment>();

            // Setup the mocked environment if required
            _mockEnv.Setup(env => env.IsDevelopment()).Returns(true); // Example setup

            _controller = new ProductsController(_mockService.Object, _mockLogger.Object, _mockEnv.Object);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOkResult()
        {
            // Arrange
            var productsList = new List<Product> {
                new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10.00m },
                new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 15.00m }
            };
            _mockService.Setup(service => service.GetAllProductsAsync()).ReturnsAsync(productsList);

            // Act
            var result = await _controller.GetAllProductsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnProducts.Count());
        }

        [Fact]
        public async Task GetProductById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId, Name = "Product 1", Description = "Description 1", Price = 10.00m };
            _mockService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProductByIdAsync(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(product, okResult.Value);
        }


        [Fact]
        public async Task AddProduct_ValidProduct_ReturnsCreatedResult()
        {
            // Arrange
            var product = new Product { Name = "New Product", Description = "New Description", Price = 15.00m };
            _mockService.Setup(service => service.AddProductAsync(product)).Verifiable();

            // Act
            var result = await _controller.AddProductAsync(product);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(product, createdAtActionResult.Value);
            _mockService.Verify(service => service.AddProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task AddProduct_InvalidProduct_ReturnsBadRequest()
        {
            // Arrange
            var product = new Product { Name = string.Empty, Price = -5.00m }; // Invalid inputs
            _controller.ModelState.AddModelError("Name", "Name is required."); // Simulate validation error
            _controller.ModelState.AddModelError("Price", "Price must be greater than zero."); // Simulate validation error

            // Act
            var result = await _controller.AddProductAsync(product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = badRequestResult.Value as SerializableError;
            Assert.NotNull(errors);
        }
    }
}