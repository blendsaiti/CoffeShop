using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class UserProfile
    {
        public int UserProfileID { get; set; } // ID e profilit të përdoruesit

        [Required]
        public string UserID { get; set; } // ID e përdoruesit (lidhje me Identity User)

        [StringLength(100)]
        public string? FirstName { get; set; } // Emri

        [StringLength(100)]
        public string? LastName { get; set; } // Mbiemri

        [Phone]
        public string? Phone { get; set; } // Numri i telefonit

        [StringLength(200)]
        public string? Address { get; set; } // Adresa

        [StringLength(100)]
        public string? City { get; set; } // Qyteti

        [StringLength(20)]
        public string? PostalCode { get; set; } // Kodi postar

        [StringLength(100)]
        public string? Country { get; set; } // Shteti

        public DateTime? DateOfBirth { get; set; } // Data e lindjes

        public string? ProfileImageUrl { get; set; } // URL e imazhit të profilit

        public DateTime CreatedDate { get; set; } // Data e krijimit të profilit

        public DateTime? LastUpdated { get; set; } // Data e përditësimit të fundit

        // Navigation properties
        public List<Order>? Orders { get; set; } // Porosite e përdoruesit
        public List<Review>? Reviews { get; set; } // Review-t e përdoruesit
    }
}