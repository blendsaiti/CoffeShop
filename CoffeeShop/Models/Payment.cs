using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class Payment
    {
        public int PaymentID { get; set; } // ID e pagesës

        [Required]
        public int OrderID { get; set; } // ID e porosisë për të cilën është pagesa
        public Order? Order { get; set; } // Porosia për të cilën është pagesa

        [Required]
        public string PaymentMethod { get; set; } // Metoda e pagesës (CreditCard, PayPal, Cash)

        [Required]
        public decimal Amount { get; set; } // Shuma e paguar

        public DateTime PaymentDate { get; set; } // Data e pagesës

        public string PaymentStatus { get; set; } // Statusi i pagesës (Pending, Completed, Failed, Refunded)

        public string? TransactionID { get; set; } // ID e transaksionit nga payment gateway

        // Për Credit Card
        [StringLength(4)]
        public string? LastFourDigits { get; set; } // 4 shifrat e fundit të kartës

        public string? CardType { get; set; } // Tipi i kartës (Visa, MasterCard, etc.)
    }
}