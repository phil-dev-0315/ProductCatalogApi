using ProductCatalogApi.Models;

namespace ProductCatalogApi.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id); // Updated to async
        Task AddProductAsync(Product product); // Updated to async
    }
}
    