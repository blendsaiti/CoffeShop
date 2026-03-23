using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class Address
    {
        public int AddressID { get; set; } // ID e adresës

        [Required]
        public string UserID { get; set; } // ID e përdoruesit

        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; } // Emri

        [Required]
        [StringLength(100)]
        public string? LastName { get; set; } // Mbiemri

        [Required]
        [Phone]
        public string? Phone { get; set; } // Telefoni

        [Required]
        [StringLength(200)]
        public string? StreetAddress { get; set; } // Adresa e rrugës

        [Required]
        [StringLength(100)]
        public string? City { get; set; } // Qyteti

        [StringLength(20)]
        public string? PostalCode { get; set; } // Kodi postar

        [Required]
        [StringLength(100)]
        public string? Country { get; set; } // Shteti

        public bool IsDefault { get; set; } = false; // Nëse është adresa default

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Data e krijimit
    }
}