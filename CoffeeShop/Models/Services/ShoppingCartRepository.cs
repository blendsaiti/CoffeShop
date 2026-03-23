using CoffeeShop.Data;
using CoffeeShop.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Models.Services
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private CoffeeShopDbContext dbContext;
        public ShoppingCartRepository(CoffeeShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public List<ShoppingCartitem>? ShoppingCartitems { get; set; }

        public string? shoppingCartId { get; set; }

        public static ShoppingCartRepository GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            CoffeeShopDbContext context = services.GetService<CoffeeShopDbContext>() ?? throw new Exception("Error initalizing coffeshopdbcontext");
            string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();
            session?.SetString("CartId", cartId);
            return new ShoppingCartRepository(context) { shoppingCartId = cartId };
        }

        public void AddToCart(Product product)
        {

            var shoppingCartItem = dbContext.ShoppingCartitems.FirstOrDefault(s => s.Product.ProductID == product.ProductID && s.ShoppingCartID == shoppingCartId);
            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartitem()
                {
                    ShoppingCartID = shoppingCartId,
                    Product = product,
                    Qty = 1
                };
                dbContext.ShoppingCartitems.Add(shoppingCartItem);

            }
            else
            {
                shoppingCartItem.Qty++;
            }
            dbContext.SaveChanges();
        }

        public void ClearCart()
        {
            var carItems = dbContext.ShoppingCartitems.Where(s => s.ShoppingCartID == shoppingCartId);
            dbContext.ShoppingCartitems.RemoveRange(carItems);
            dbContext.SaveChanges();
        }

        public List<ShoppingCartitem> GetShoppingCartitems()
        {
            return ShoppingCartitems ??= dbContext.ShoppingCartitems.Where(s => s.ShoppingCartID == shoppingCartId).Include(p => p.Product).ToList();

        }

        public decimal GetShoppingCartTotal()
        {
            var totalCost = dbContext.ShoppingCartitems.Where(s => s.ShoppingCartID == shoppingCartId).Select(s => s.Product.Price * s.Qty).Sum();
            return totalCost;
        }

        public int RemoveFromCart(Product product)
        {
            var shoppingCartItem = dbContext.ShoppingCartitems.FirstOrDefault(s => s.Product.ProductID == product.ProductID && s.ShoppingCartID == shoppingCartId);
            var quantity = 0;
            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Qty > 1)
                {
                    shoppingCartItem.Qty--;
                    quantity = shoppingCartItem.Qty;
                }
                else
                {
                    dbContext.ShoppingCartitems.Remove(shoppingCartItem);

                }

            }
            dbContext.SaveChanges();
            return quantity;
        }
    }
}
