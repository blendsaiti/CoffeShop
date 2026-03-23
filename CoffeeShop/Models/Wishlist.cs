using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class Wishlist
    {
        public int WishlistID { get; set; } // ID e wishlist-it

        [Required]
        public string UserID { get; set; } // ID e përdoruesit

        [Required]
        public int ProductID { get; set; } // ID e produktit
        public Product? Product { get; set; } // Produkti

        public DateTime AddedDate { get; set; } = DateTime.Now; // Data kur është shtuar
    }
}