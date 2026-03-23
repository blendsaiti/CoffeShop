using CoffeeShop.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private IShoppingCartRepository shoppingCartRepository;
        private IProductRepository productRepository;

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository, IProductRepository productRepository)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var items = shoppingCartRepository.GetShoppingCartitems();
            shoppingCartRepository.ShoppingCartitems = items;

            ViewBag.CartTotal = shoppingCartRepository.GetShoppingCartTotal();

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToShoppingCart(int? pId, int? productId)
        {
            int id = pId ?? productId ?? 0;

            if (id == 0)
            {
                TempData["Error"] = "Invalid product ID";
                return RedirectToAction("Index");
            }

            var product = productRepository.GetAllProducts().FirstOrDefault(p => p.ProductID == id);

            if (product != null)
            {
                shoppingCartRepository.AddToCart(product);
                TempData["Success"] = $"{product.Name} added to cart!";
                int cartCount = shoppingCartRepository.GetShoppingCartitems().Count;
                HttpContext.Session.SetInt32("cartCount", cartCount);
            }

            return Ok();
        }

        public RedirectToActionResult RemoveFromShoppingCart(int pId)
        {
            var product = productRepository.GetAllProducts().FirstOrDefault(p => p.ProductID == pId);
            if (product != null)
            {
                shoppingCartRepository.RemoveFromCart(product);
                int cartCount = shoppingCartRepository.GetShoppingCartitems().Count;
                HttpContext.Session.SetInt32("cartCount", cartCount);
            }
            return RedirectToAction("Index");
        }
    }
}
