using Microsoft.Extensions.Caching.Memory;
using ProductCatalogApi.Data;
using ProductCatalogApi.Models;

namespace ProductCatalogApi.Services
{
    public class ProductService : IProductService
    {
        private readonly StoreHubContext _context;
        private readonly IMemoryCache _cache;
        public ProductService(StoreHubContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public IEnumerable<Product> GetAllProducts()
        {
            if (!_cache.TryGetValue("CachedProducts", out IEnumerable<Product> products))
            {
                products = _context.Products.ToList();

                // Set cache options
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                // Save data in cache
                _cache.Set("CachedProducts", products, cacheEntryOptions);
            }

            return products;
        }
        public Product? GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }
        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            // Invalidate cache
            _cache.Remove("CachedProducts");
        }
    }
}
