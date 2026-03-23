using CoffeeShop.Data;
using CoffeeShop.Models.Interfaces;
using CoffeeShop.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Models.Services
{
    public class ProductRepository : IProductRepository
    {
        private CoffeeShopDbContext dbContext;

        public ProductRepository(CoffeeShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ToList();
        }

        public Product GetProductDetail(int id)
        {
            return dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews.Where(r => r.IsApproved))
                .FirstOrDefault(p => p.ProductID == id);
        }

        public IEnumerable<Product> GetTrendingProducts()
        {
            return dbContext.Products
                .Where(p => p.IsTrendingProduct && p.IsAvailable)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ToList();
        }

        public Product AddProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            product.CreatedDate = DateTime.Now;
            product.IsAvailable = true;

            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            return product;
        }

        public Product UpdateProduct(Product updatedProduct)
        {
            if (updatedProduct == null)
            {
                throw new ArgumentNullException(nameof(updatedProduct));
            }

            Product existingProduct = dbContext.Products.FirstOrDefault(p => p.ProductID == updatedProduct.ProductID);

            if (existingProduct != null)
            {
                existingProduct.Name = updatedProduct.Name;
                existingProduct.Detail = updatedProduct.Detail;
                existingProduct.ImageUrl = updatedProduct.ImageUrl;
                existingProduct.Price = updatedProduct.Price;
                existingProduct.IsTrendingProduct = updatedProduct.IsTrendingProduct;
                existingProduct.CategoryID = updatedProduct.CategoryID;
                existingProduct.StockQuantity = updatedProduct.StockQuantity;
                existingProduct.LowStockThreshold = updatedProduct.LowStockThreshold;
                existingProduct.IsAvailable = updatedProduct.IsAvailable;
                existingProduct.LastUpdated = DateTime.Now;

                dbContext.SaveChanges();
            }

            return existingProduct;
        }

        public void DeleteProduct(int productId)
        {
            Product productToDelete = dbContext.Products.FirstOrDefault(p => p.ProductID == productId);

            if (productToDelete != null)
            {
                dbContext.Products.Remove(productToDelete);
                dbContext.SaveChanges();
            }
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return dbContext.Products
                .Where(p => p.CategoryID == categoryId && p.IsAvailable)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ToList();
        }

        public IEnumerable<Product> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return GetAllProducts();
            }

            return dbContext.Products
                .Where(p => p.Name.Contains(searchTerm) || p.Detail.Contains(searchTerm))
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ToList();
        }

        public IEnumerable<Product> GetLowStockProducts()
        {
            return dbContext.Products
                .Where(p => p.StockQuantity <= p.LowStockThreshold)
                .Include(p => p.Category)
                .OrderBy(p => p.StockQuantity)
                .ToList();
        }

        public void UpdateStock(int productId, int quantity)
        {
            var product = dbContext.Products.Find(productId);
            if (product != null)
            {
                product.StockQuantity = quantity;
                product.LastUpdated = DateTime.Now;

                // Përditëso disponibilitetin bazuar në stokun
                product.IsAvailable = quantity > 0;

                dbContext.SaveChanges();
            }
        }

        public IEnumerable<ProductSalesInfo> GetTopSellingProducts(int count)
        {
            var topProducts = dbContext.OrderDetails
                .GroupBy(od => od.ProductID)
                .Select(g => new ProductSalesInfo
                {
                    Product = dbContext.Products.FirstOrDefault(p => p.ProductID == g.Key),
                    QuantitySold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.Price * od.Quantity)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(count)
                .ToList();

            return topProducts;
        }

        public IEnumerable<Product> GetAvailableProducts()
        {
            return dbContext.Products
                .Where(p => p.IsAvailable && p.StockQuantity > 0)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ToList();
        }

        public bool IsProductAvailable(int productId, int quantity)
        {
            var product = dbContext.Products.Find(productId);
            return product != null && product.IsAvailable && product.StockQuantity >= quantity;
        }
    }
}