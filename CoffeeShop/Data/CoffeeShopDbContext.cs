using CoffeeShop.Models; 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore; 

namespace CoffeeShop.Data 
{
    public class CoffeeShopDbContext : IdentityDbContext
    {
        public CoffeeShopDbContext(DbContextOptions<CoffeeShopDbContext> options) : base(options)
        {
        }

 
        public DbSet<Product> Products { get; set; } 
        public DbSet<ShoppingCartitem> ShoppingCartitems { get; set; } 
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; } 

        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Wishlist> Wishlists { get; set; }

        public DbSet<Address> Addresses { get; set; }
    }
}
