using MediatR;
using ProductCatalogApi.Models;
using ProductCatalogApi.Services;

namespace ProductCatalogApi.Commands.Products
{
    public class AddProductCommand : IRequest<Product>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Product>
    {
        private readonly IProductService _productService;

        public AddProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Product> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price
            };
            await _productService.AddProductAsync(product);
            return product;
        }
    }
}
