using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using ProductCatalogApi.Commands.Products;
using ProductCatalogApi.Controllers;
using ProductCatalogApi.Models;
using ProductCatalogApi.Queries.Products;
using ProductCatalogApi.Services;

namespace ProductCatalogApi.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<IMediator> _mockMediator;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(env => env.EnvironmentName).Returns("Development");
            _mockMediator = new Mock<IMediator>();

            _controller = new ProductsController(_mockService.Object, _mockLogger.Object, _mockEnv.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOkResult()
        {
            // Arrange
            var productsList = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10.00m },
                new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 15.00m }
            };
            _mockMediator.Setup(mediator => mediator.Send(It.IsAny<GetAllProductsQuery>(), default))
                          .ReturnsAsync(productsList);

            // Act
            var result = await _controller.GetAllProductsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnProducts.Count());
        }

        [Fact]
        public async Task GetProductByIdAsync_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId, Name = "Product 1", Description = "Description 1", Price = 10.00m };
            _mockMediator.Setup(mediator => mediator.Send(It.IsAny<GetProductByIdQuery>(), default))
                          .ReturnsAsync(product);

            // Act
            var result = await _controller.GetProductByIdAsync(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public async Task AddProductAsync_ValidProduct_ReturnsCreatedResult()
        {
            // Arrange
            var product = new Product { Name = "New Product", Description = "New Description", Price = 15.00m };
            var addedProduct = new Product { Id = 3, Name = "New Product", Description = "New Description", Price = 15.00m }; // Simulating the added product with an ID

            _mockMediator.Setup(mediator => mediator.Send(It.IsAny<AddProductCommand>(), default))
                          .ReturnsAsync(addedProduct); // Mocking MediatR behavior for product addition.

            // Act
            var result = await _controller.AddProductAsync(product);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(addedProduct.Id, ((Product)createdAtActionResult.Value).Id);
            Assert.Equal("New Product", ((Product)createdAtActionResult.Value).Name);
        }

        [Fact]
        public async Task AddProductAsync_InvalidProduct_ReturnsBadRequest()
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
            Assert.NotNull(errors); // Ensure errors are present
        }
    }
}