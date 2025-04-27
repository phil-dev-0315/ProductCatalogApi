using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductCatalogApi.Data;
using ProductCatalogApi.Models;
using ProductCatalogApi.Repositories;
using Serilog;

namespace ProductCatalogApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMemoryCache _cache;
        public ProductService(IProductRepository productRepository, IMemoryCache cache)
        {
            _productRepository = productRepository;
            _cache = cache;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                if (!_cache.TryGetValue("CachedProducts", out IEnumerable<Product> products))
                {
                    products = await _productRepository.GetAllAsync();

                    // Set cache options
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    // Save data in cache
                    _cache.Set("CachedProducts", products, cacheEntryOptions);
                }

                return products;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error($"ProductService.GetAllProductsAsync(). Database error: {dbEx.InnerException?.Message}");

                throw new Exception("A database error occurred while retrieving products. Please try again.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"ProductService.GetAllProductsAsync(). Error message: {ex.Message} Stacktrace: {ex.StackTrace}");
                throw new Exception("An error occurred while retrieving products. Please try again.");
            }
        }
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                return await _productRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ProductService.GetProductByIdAsync(). Error retrieving product by ID: {ProductId}", id);
                throw new Exception("An error occurred while retrieving the product. Please try again.");
            }
        }

        public async Task AddProductAsync(Product product)
        {
            try
            {
                await _productRepository.AddAsync(product);

                Log.Information("Product added successfully: {@Product}", product);
                _cache.Remove("CachedProducts");
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error($"ProductService.AddProductAsync(). Database update failed: {dbEx.InnerException?.Message}");
                throw new Exception("A database error occurred while adding the product. Please try again.");
            }
            catch (Exception ex)
            {
                Log.Error($"ProductService.AddProductAsync(). Error: {ex.Message} Unexpected error while adding product: {product}");
                throw new Exception("An error occurred while adding the product. Please try again.");
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                await _productRepository.UpdateAsync(product);
                _cache.Remove("CachedProducts"); // Invalidate cache
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error($"ProductService.UpdateProductAsync(). Database error while updating product: {dbEx.InnerException?.Message}");
                throw new Exception("A database error occurred while updating the product. Please try again.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while updating product.");
                throw new Exception("An error occurred while updating the product. Please try again.");
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            try
            {
                await _productRepository.DeleteAsync(id);
                _cache.Remove("CachedProducts"); // Invalidate cache
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error($"ProductService.DeleteProductAsync(). Database error while deleting product with ID: {id}. Error: {dbEx.InnerException?.Message}");
                throw new Exception("A database error occurred while deleting the product. Please try again.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while deleting product.");
                throw new Exception("An error occurred while deleting the product. Please try again.");
            }
        }
    }
}
