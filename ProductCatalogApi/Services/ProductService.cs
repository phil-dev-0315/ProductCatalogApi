using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductCatalogApi.Data;
using ProductCatalogApi.Models;
using Serilog;

namespace ProductCatalogApi.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductCatalogContext _context;
        private readonly IMemoryCache _cache;
        public ProductService(ProductCatalogContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                if (!_cache.TryGetValue("CachedProducts", out IEnumerable<Product> products))
                {
                    products = await _context.Products.ToListAsync();

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
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }
                return product; // Null return can be handle by the consumer.
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
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

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
    }
}
