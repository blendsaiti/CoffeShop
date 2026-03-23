using CoffeeShop.Data;
using CoffeeShop.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Models.Services
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly CoffeeShopDbContext dbContext;

        public WishlistRepository(CoffeeShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void AddToWishlist(string userId, int productId)
        {
            // Kontrollo nëse produkti është tashmë në wishlist
            if (!IsInWishlist(userId, productId))
            {
                var wishlistItem = new Wishlist
                {
                    UserID = userId,
                    ProductID = productId,
                    AddedDate = DateTime.Now
                };

                dbContext.Wishlists.Add(wishlistItem);
                dbContext.SaveChanges();
            }
        }

        public void RemoveFromWishlist(string userId, int productId)
        {
            var wishlistItem = dbContext.Wishlists
                .FirstOrDefault(w => w.UserID == userId && w.ProductID == productId);

            if (wishlistItem != null)
            {
                dbContext.Wishlists.Remove(wishlistItem);
                dbContext.SaveChanges();
            }
        }

        public IEnumerable<Wishlist> GetUserWishlist(string userId)
        {
            return dbContext.Wishlists
                .Where(w => w.UserID == userId)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Category)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Reviews)
                .OrderByDescending(w => w.AddedDate)
                .ToList();
        }

        public bool IsInWishlist(string userId, int productId)
        {
            return dbContext.Wishlists
                .Any(w => w.UserID == userId && w.ProductID == productId);
        }

        public void ClearWishlist(string userId)
        {
            var wishlistItems = dbContext.Wishlists
                .Where(w => w.UserID == userId);

            dbContext.Wishlists.RemoveRange(wishlistItems);
            dbContext.SaveChanges();
        }

        public int GetWishlistCount(string userId)
        {
            return dbContext.Wishlists
                .Count(w => w.UserID == userId);
        }
    }
}