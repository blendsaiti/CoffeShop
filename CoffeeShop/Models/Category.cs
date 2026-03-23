using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class Category
    {
        public int CategoryID { get; set; } // ID e kategorisë

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Emri i kategorisë

        [StringLength(500)]
        public string? Description { get; set; } // Përshkrimi i kategorisë

        public string? ImageUrl { get; set; } // URL e imazhit të kategorisë

        public bool IsActive { get; set; } = true; // Nëse kategoria është aktive

        public int DisplayOrder { get; set; } // Radhitja e shfaqjes

        // Navigation property
        public List<Product>? Products { get; set; } // Produktet në këtë kategori
    }
}