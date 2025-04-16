using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductCatalogApi.Controllers;
using ProductCatalogApi.Models;
using ProductCatalogApi.Services;

namespace StoreHubApiTest
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductsController(_mockService.Object);
        }

        [Fact]
        public void GetProducts_ReturnsOkResult()
        {
            // Arrange
            _mockService.Setup(service => service.GetAllProducts()).Returns(new List<Product>());

            // Act
            var result = _controller.GetAllProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        }

        [Fact]
        public void GetProduct_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId, Name = "Product 1", Description = "Description 1", Price = 10.00m };
            _mockService.Setup(service => service.GetProductById(productId)).Returns(product);

            // Act
            var result = _controller.GetProductById(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public void CreateProduct_ValidProduct_ReturnsCreatedResult()
        {
            // Arrange
            var product = new Product { Name = "New Product", Description = "New Description", Price = 15.00m };

            // Act
            var result = _controller.AddProduct(product);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(product, createdAtActionResult.Value);
        }

        [Fact]
        public void CreateProduct_InvalidProduct_ReturnsBadRequest()
        {
            // Arrange
            var product = new Product { Name = string.Empty, Price = -5.00m }; // Invalid inputs
            _controller.ModelState.AddModelError("Name", "Name is required.");
            _controller.ModelState.AddModelError("Price", "Price must be greater than zero.");

            // Act
            var result = _controller.AddProduct(product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = badRequestResult.Value as SerializableError;
            Assert.NotNull(errors);
        }
    }
}