using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class Review
    {
        public int ReviewID { get; set; } // ID e review-it

        [Required]
        public int ProductID { get; set; } // ID e produktit që është reviewuar
        public Product? Product { get; set; } // Produkti që është reviewuar

        [Required]
        public string UserID { get; set; } // ID e përdoruesit që ka bërë review-in

        [Required]
        [StringLength(100)]
        public string? UserName { get; set; } // Emri i përdoruesit që ka bërë review-in

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; } // Vlerësimi nga 1 deri në 5

        [StringLength(1000)]
        public string? Comment { get; set; } // Komenti i review-it

        public DateTime ReviewDate { get; set; } // Data e review-it

        public bool IsApproved { get; set; } // Nëse review-i është aprovuar nga admin-i
    }
}