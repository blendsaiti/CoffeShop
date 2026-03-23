using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class Product
    {
        public int ProductID { get; set; } // Identifikues unik për produktin

        [Required]
        [StringLength(200)]
        public string? Name { get; set; } // Emri i produktit

        [StringLength(1000)]
        public string? Detail { get; set; } // Përshkrimi ose detajet e produktit

        public string? ImageUrl { get; set; } // Rruga e imazhit të produktit

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; } // Çmimi i produktit

        public bool IsTrendingProduct { get; set; } // Nëse produkti është i trenduar

        // Kategoria
        public int? CategoryID { get; set; } // ID e kategorisë (nullable për backwards compatibility)
        public Category? Category { get; set; } // Kategoria e produktit

        // Inventory Management
        public int StockQuantity { get; set; } = 0; // Sasia në stok

        public int LowStockThreshold { get; set; } = 10; // Kufiri i ulët i stokut

        public bool IsAvailable { get; set; } = true; // Nëse produkti është i disponueshëm

        // Metadata
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Data e krijimit

        public DateTime? LastUpdated { get; set; } // Data e përditësimit të fundit

        // Navigation properties
        public List<Review>? Reviews { get; set; } // Review-t e produktit

        // Calculated properties
        public bool IsLowStock => StockQuantity <= LowStockThreshold; // Nëse produkti ka stok të ulët

        public double AverageRating => Reviews?.Where(r => r.IsApproved).Any() == true
            ? Reviews.Where(r => r.IsApproved).Average(r => r.Rating)
            : 0; // Mesatarja e vlerësimeve

        public int ReviewCount => Reviews?.Count(r => r.IsApproved) ?? 0; // Numri i review-ve
    }
}