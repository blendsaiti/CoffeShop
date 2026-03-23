using System.Net;
using System.Net.Mail;

namespace CoffeeShop.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmationEmailAsync(string toEmail, string customerName, int orderId, decimal total);
        Task SendOrderStatusUpdateEmailAsync(string toEmail, string customerName, int orderId, string newStatus);
        Task SendOrderCompletedEmailAsync(string toEmail, string customerName, int orderId, decimal total);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendOrderConfirmationEmailAsync(string toEmail, string customerName, int orderId, decimal total)
        {
            var subject = $"Order Confirmation - Order #{orderId}";
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #d7a86e 0%, #b8884a 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .order-details {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; }}
                        .total {{ font-size: 24px; color: #d7a86e; font-weight: bold; }}
                        .footer {{ text-align: center; color: #777; font-size: 12px; margin-top: 30px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>☕ Order Confirmed!</h1>
                        </div>
                        <div class='content'>
                            <p>Hi {customerName},</p>
                            <p>Thank you for your order! We've received your order and we're getting it ready.</p>
                            
                            <div class='order-details'>
                                <h3>Order Details</h3>
                                <p><strong>Order Number:</strong> #{orderId}</p>
                                <p><strong>Total Amount:</strong> <span class='total'>${total:F2}</span></p>
                                <p><strong>Status:</strong> Pending</p>
                            </div>

                            <p>We'll send you another email when your order ships.</p>
                            <p>If you have any questions, please don't hesitate to contact us.</p>
                            
                            <p>Best regards,<br>The CoffeeShop Team</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2024 CoffeeShop. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderStatusUpdateEmailAsync(string toEmail, string customerName, int orderId, string newStatus)
        {
            var subject = $"Order Update - Order #{orderId}";
            var statusMessage = GetStatusMessage(newStatus);

            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #d7a86e 0%, #b8884a 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .status-badge {{ display: inline-block; background: #d7a86e; color: white; padding: 10px 20px; border-radius: 20px; font-weight: bold; margin: 20px 0; }}
                        .footer {{ text-align: center; color: #777; font-size: 12px; margin-top: 30px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>📦 Order Status Updated</h1>
                        </div>
                        <div class='content'>
                            <p>Hi {customerName},</p>
                            <p>Your order status has been updated!</p>
                            
                            <div style='text-align: center;'>
                                <p><strong>Order #{orderId}</strong></p>
                                <div class='status-badge'>{newStatus}</div>
                            </div>

                            <p>{statusMessage}</p>
                            
                            <p>Thank you for shopping with us!</p>
                            
                            <p>Best regards,<br>The CoffeeShop Team</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2024 CoffeeShop. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderCompletedEmailAsync(string toEmail, string customerName, int orderId, decimal total)
        {
            var subject = $"Order Delivered - Order #{orderId}";

            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .checkmark {{ font-size: 64px; color: #48bb78; text-align: center; margin: 20px 0; }}
                        .cta-button {{ display: inline-block; background: #d7a86e; color: white; padding: 12px 30px; text-decoration: none; border-radius: 8px; margin: 20px 0; }}
                        .footer {{ text-align: center; color: #777; font-size: 12px; margin-top: 30px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🎉 Order Delivered!</h1>
                        </div>
                        <div class='content'>
                            <div class='checkmark'>✓</div>
                            
                            <p>Hi {customerName},</p>
                            <p><strong>Great news! Your order has been delivered.</strong></p>
                            
                            <p><strong>Order Number:</strong> #{orderId}</p>
                            <p><strong>Total Amount:</strong> ${total:F2}</p>
                            
                            <p>We hope you enjoy your purchase! If you have any questions or concerns, please don't hesitate to contact us.</p>
                            
                            <div style='text-align: center;'>
                                <a href='#' class='cta-button'>Leave a Review</a>
                            </div>
                            
                            <p>Thank you for choosing CoffeeShop!</p>
                            
                            <p>Best regards,<br>The CoffeeShop Team</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2024 CoffeeShop. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var fromEmail = configuration["Email:From"];
                var fromName = configuration["Email:FromName"];
                var smtpHost = configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
                var smtpUsername = configuration["Email:SmtpUsername"];
                var smtpPassword = configuration["Email:SmtpPassword"];

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(fromEmail, fromName);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = true;

                        await smtpClient.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        private string GetStatusMessage(string status)
        {
            return status switch
            {
                "Processing" => "Your order is being prepared and will be shipped soon.",
                "Shipped" => "Your order has been shipped and is on its way to you!",
                "Completed" => "Your order has been delivered. We hope you enjoy your purchase!",
                "Cancelled" => "Your order has been cancelled. If you have any questions, please contact us.",
                _ => "Your order status has been updated."
            };
        }
    }
}
