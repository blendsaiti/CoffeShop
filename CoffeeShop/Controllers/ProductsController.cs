using CoffeeShop.Data;
using CoffeeShop.Models;
using CoffeeShop.Models.Interfaces;
using CoffeeShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly CoffeeShopDbContext dbContext;

        public ProductsController(IProductRepository productRepository, CoffeeShopDbContext dbContext)
        {
            this.productRepository = productRepository;
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Shop(
            string? searchTerm,
            int? categoryId,
            string? sortBy,
            decimal? minPrice,
            decimal? maxPrice,
            int page = 1,
            int pageSize = 9)
        {
            var query = dbContext.Products
                .Include(p => p.Category)
                .Where(p => p.IsAvailable)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Detail.Contains(searchTerm));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            query = sortBy switch
            {
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.ProductID),
                _ => query.OrderBy(p => p.Name)
            };

            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await dbContext.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            var viewModel = new ProductListViewModel
            {
                Products = products,
                Categories = categories,
                SearchTerm = searchTerm,
                SelectedCategoryId = categoryId,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CurrentPage = page,
                PageSize = pageSize,
                TotalProducts = totalProducts,
                TotalPages = totalPages
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ShopResultsPartial", viewModel);
            }

            return View(viewModel);
        }

        public IActionResult Detail(int id)
        {
            var product = productRepository.GetProductDetail(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product newProduct)
        {
            if (!ModelState.IsValid)
            {
                return View(newProduct);
            }

            productRepository.AddProduct(newProduct);
            return RedirectToAction("Shop");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = productRepository.GetProductDetail(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product updatedProduct)
        {
            if (id != updatedProduct.ProductID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(updatedProduct);
            }

            var existingProduct = productRepository.GetProductDetail(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Detail = updatedProduct.Detail;
            existingProduct.ImageUrl = updatedProduct.ImageUrl;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.IsTrendingProduct = updatedProduct.IsTrendingProduct;
            existingProduct.StockQuantity = updatedProduct.StockQuantity;
            existingProduct.IsAvailable = updatedProduct.StockQuantity > 0;
            existingProduct.CategoryID = updatedProduct.CategoryID;

            productRepository.UpdateProduct(existingProduct);

            return RedirectToAction("Shop");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            productRepository.DeleteProduct(id);
            return RedirectToAction("Shop");
        }
    }
}