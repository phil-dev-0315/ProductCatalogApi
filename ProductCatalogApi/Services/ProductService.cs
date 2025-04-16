using ProductCatalogApi.Data;
using ProductCatalogApi.Models;

namespace ProductCatalogApi.Services
{
    public class ProductService : IProductService
    {
        private readonly StoreHubContext _context;
        public ProductService(StoreHubContext context)
        {
            _context = context;
        }
        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }
        public Product? GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }
        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }
    }
}
