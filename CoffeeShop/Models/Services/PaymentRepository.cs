using CoffeeShop.Data;
using CoffeeShop.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Models.Services
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly CoffeeShopDbContext dbContext;

        public PaymentRepository(CoffeeShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Payment CreatePayment(Payment payment)
        {
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            payment.PaymentDate = DateTime.Now;
            payment.PaymentStatus = "Pending";

            dbContext.Payments.Add(payment);
            dbContext.SaveChanges();

            return payment;
        }

        public Payment? GetPaymentById(int paymentId)
        {
            return dbContext.Payments
                .Include(p => p.Order)
                .FirstOrDefault(p => p.PaymentID == paymentId);
        }

        public Payment? GetPaymentByOrderId(int orderId)
        {
            return dbContext.Payments
                .Include(p => p.Order)
                .FirstOrDefault(p => p.OrderID == orderId);
        }

        public void UpdatePaymentStatus(int paymentId, string status, string? transactionId = null)
        {
            var payment = dbContext.Payments.FirstOrDefault(p => p.PaymentID == paymentId);

            if (payment != null)
            {
                payment.PaymentStatus = status;
                if (!string.IsNullOrEmpty(transactionId))
                {
                    payment.TransactionID = transactionId;
                }
                dbContext.SaveChanges();
            }
        }

        public IEnumerable<Payment> GetAllPayments()
        {
            return dbContext.Payments
                .Include(p => p.Order)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
        }

        public IEnumerable<Payment> GetPaymentsByStatus(string status)
        {
            return dbContext.Payments
                .Where(p => p.PaymentStatus == status)
                .Include(p => p.Order)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
        }

        public async Task<PaymentResult> ProcessCreditCardPayment(CreditCardPaymentInfo cardInfo, decimal amount, int orderId)
        {
            // Ky është një simulim i procesimit të pagesës
            // Në një aplikacion real, këtu do të integrohej me një payment gateway si Stripe, PayPal, etj.

            try
            {
                // Validimi i kartës
                if (string.IsNullOrEmpty(cardInfo.CardNumber) || cardInfo.CardNumber.Length < 13)
                {
                    return new PaymentResult
                    {
                        Success = false,
                        Message = "Numri i kartës nuk është i vlefshëm"
                    };
                }

                if (string.IsNullOrEmpty(cardInfo.CVV) || cardInfo.CVV.Length < 3)
                {
                    return new PaymentResult
                    {
                        Success = false,
                        Message = "CVV nuk është i vlefshëm"
                    };
                }

                // Simulim i procesimit (në realitet do të kishte një API call)
                await Task.Delay(1000); // Simulon kohën e procesimit

                // Krijo pagesën
                var payment = new Payment
                {
                    OrderID = orderId,
                    PaymentMethod = "CreditCard",
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Completed",
                    TransactionID = "TXN" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    LastFourDigits = cardInfo.CardNumber.Substring(cardInfo.CardNumber.Length - 4),
                    CardType = DetermineCardType(cardInfo.CardNumber)
                };

                dbContext.Payments.Add(payment);
                dbContext.SaveChanges();

                return new PaymentResult
                {
                    Success = true,
                    Message = "Pagesa u krye me sukses",
                    TransactionId = payment.TransactionID,
                    Payment = payment
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    Success = false,
                    Message = "Pagesa dështoi: " + ex.Message
                };
            }
        }

        private string DetermineCardType(string cardNumber)
        {
            // Logjika e thjeshtë për të përcaktuar tipin e kartës
            if (cardNumber.StartsWith("4"))
                return "Visa";
            else if (cardNumber.StartsWith("5"))
                return "MasterCard";
            else if (cardNumber.StartsWith("3"))
                return "American Express";
            else
                return "Unknown";
        }
    }
}