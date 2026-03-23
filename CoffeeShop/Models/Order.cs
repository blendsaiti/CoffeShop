using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class Order
    {
        public int OrderID { get; set; } // ID e porosisë

        [Required]
        public string UserID { get; set; } // ID e përdoruesit që ka bërë porosinë

        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; } // Emri i personit që ka bërë porosinë

        [Required]
        [StringLength(100)]
        public string? LastName { get; set; } // Mbiemri i personit që ka bërë porosinë

        [Required]
        [EmailAddress]
        public string? Email { get; set; } // Adresa email e personit që ka bërë porosinë

        [Required]
        [Phone]
        public string? Phone { get; set; } // Numri i telefonit të personit që ka bërë porosinë

        [Required]
        [StringLength(200)]
        public string? Address { get; set; } // Adresa e dërgesës së porosisë

        [StringLength(100)]
        public string? City { get; set; } // Qyteti

        [StringLength(20)]
        public string? PostalCode { get; set; } // Kodi postar

        [StringLength(100)]
        public string? Country { get; set; } // Shteti

        [Required]
        public decimal OrderTotal { get; set; } // Totali i çmimit të porosisë

        public DateTime OrderPlaced { get; set; } // Data dhe koha kur porosia është vendosur

        public string OrderStatus { get; set; } = "Pending"; // Statusi i porosisë (Pending, Processing, Shipped, Delivered, Cancelled)

        public DateTime? ShippedDate { get; set; } // Data e dërgimit

        public DateTime? DeliveredDate { get; set; } // Data e dorëzimit

        public string? TrackingNumber { get; set; } // Numri i gjurmimit

        public string? Notes { get; set; } // Shënime për porosinë

        // Navigation properties
        public List<OrderDetail>? OrderDetails { get; set; } // Lista e detajeve të porosisë

        public Payment? Payment { get; set; } // Pagesa e porosisë
    }
}