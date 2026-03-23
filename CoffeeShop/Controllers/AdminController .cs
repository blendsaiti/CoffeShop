using CoffeeShop.Models.Interfaces;
using CoffeeShop.Services;
using CoffeeShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IReviewRepository reviewRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IEmailService emailService;

        public AdminController(
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IReviewRepository reviewRepository,
            IPaymentRepository paymentRepository,
            IEmailService emailService)
        {
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.reviewRepository = reviewRepository;
            this.paymentRepository = paymentRepository;
            this.emailService = emailService;
        }

        public IActionResult Dashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalProducts = productRepository.GetAllProducts().Count(),
                TotalOrders = orderRepository.GetAllOrders().Count(),
                PendingReviews = reviewRepository.GetPendingReviews().Count(),
                TotalRevenue = orderRepository.GetTotalRevenue(),
                RecentOrders = orderRepository.GetRecentOrders(10),
                LowStockProducts = productRepository.GetLowStockProducts(),
                PendingPayments = paymentRepository.GetPaymentsByStatus("Pending").Count()
            };

            return View(viewModel);
        }

        public IActionResult Products()
        {
            var products = productRepository.GetAllProducts();
            return View(products);
        }

        public IActionResult Orders()
        {
            var orders = orderRepository.GetAllOrders();
            return View(orders);
        }

        public IActionResult OrderDetails(int id)
        {
            var order = orderRepository.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            try
            {
                var order = orderRepository.GetOrderById(orderId);

                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Orders");
                }

                var oldStatus = order.OrderStatus;

                orderRepository.UpdateOrderStatus(orderId, status);

                if (status == "Shipped" && order.ShippedDate == null)
                {
                    order.ShippedDate = DateTime.Now;
                }
                else if (status == "Completed" && order.DeliveredDate == null)
                {
                    order.DeliveredDate = DateTime.Now;
                }

                try
                {
                    if (status == "Completed" || status == "Delivered")
                    {
                        await emailService.SendOrderCompletedEmailAsync(
                            order.Email,
                            $"{order.FirstName} {order.LastName}",
                            order.OrderID,
                            order.OrderTotal
                        );
                    }
                    else if (oldStatus != status)
                    {
                        await emailService.SendOrderStatusUpdateEmailAsync(
                            order.Email,
                            $"{order.FirstName} {order.LastName}",
                            order.OrderID,
                            status
                        );
                    }
                }
                catch (Exception emailEx)
                {
                    Console.WriteLine($"Error sending email: {emailEx.Message}");
                }

                TempData["Success"] = $"Order status updated to {status}. Email notification sent to customer.";
                return RedirectToAction("OrderDetails", new { id = orderId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating order status: {ex.Message}";
                return RedirectToAction("OrderDetails", new { id = orderId });
            }
        }

        public IActionResult Reviews()
        {
            var reviews = reviewRepository.GetAllReviews();
            return View(reviews);
        }

        [HttpPost]
        public IActionResult ApproveReview(int reviewId)
        {
            reviewRepository.ApproveReview(reviewId);
            return RedirectToAction("Reviews");
        }

        [HttpPost]
        public IActionResult DeleteReview(int reviewId)
        {
            reviewRepository.DeleteReview(reviewId);
            return RedirectToAction("Reviews");
        }

        public IActionResult Payments()
        {
            var payments = paymentRepository.GetAllPayments();
            return View(payments);
        }

        public IActionResult SalesReport()
        {
            var reportData = new SalesReportViewModel
            {
                DailySales = orderRepository.GetDailySales(30),
                TopProducts = productRepository.GetTopSellingProducts(10),
                RevenueByMonth = orderRepository.GetMonthlyRevenue(12)
            };

            return View(reportData);
        }

        public IActionResult Users()
        {
            return View();
        }
    }
}