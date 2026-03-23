namespace CoffeeShop.Models.Interfaces
{
    public interface IPaymentRepository
    {
        // Krijo një pagesë të re
        Payment CreatePayment(Payment payment);

        // Merr pagesën sipas ID-së
        Payment? GetPaymentById(int paymentId);

        // Merr pagesën sipas ID-së së porosisë
        Payment? GetPaymentByOrderId(int orderId);

        // Përditëso statusin e pagesës
        void UpdatePaymentStatus(int paymentId, string status, string? transactionId = null);

        // Merr të gjitha pagesat
        IEnumerable<Payment> GetAllPayments();

        // Merr pagesat sipas statusit
        IEnumerable<Payment> GetPaymentsByStatus(string status);

        // Procezo pagesën me kartë krediti
        Task<PaymentResult> ProcessCreditCardPayment(CreditCardPaymentInfo cardInfo, decimal amount, int orderId);
    }

    // Klasa për rezultatin e pagesës
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? TransactionId { get; set; }
        public Payment? Payment { get; set; }
    }

    // Klasa për informacionin e kartës së kreditit
    public class CreditCardPaymentInfo
    {
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CVV { get; set; }
    }
}