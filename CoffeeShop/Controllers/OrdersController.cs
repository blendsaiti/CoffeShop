using CoffeeShop.Models;
using CoffeeShop.Models.Interfaces;
using CoffeeShop.Services;
using CoffeeShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace CoffeeShop.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly IShoppingCartRepository shopCartRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IAddressRepository addressRepository;
        private readonly IConfiguration configuration;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IEmailService emailService;

        public OrdersController(
            IOrderRepository orderRepository,
            IShoppingCartRepository shopCartRepository,
            IPaymentRepository paymentRepository,
            IAddressRepository addressRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            IEmailService emailService)
        {
            this.orderRepository = orderRepository;
            this.shopCartRepository = shopCartRepository;
            this.paymentRepository = paymentRepository;
            this.addressRepository = addressRepository;
            this.configuration = configuration;
            this.userManager = userManager;
            this.emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = shopCartRepository.GetShoppingCartitems();
            var savedAddresses = addressRepository.GetUserAddresses(userId);

            var user = await userManager.GetUserAsync(User);

            var model = new CheckoutViewModel
            {
                CartItems = cartItems.ToList(),
                CartTotal = cartItems.Sum(i => i.Product.Price * i.Qty),
                SavedAddresses = savedAddresses,
                StripePublishableKey = configuration["Stripe:PublishableKey"] ?? "",
                Email = user?.Email ?? ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ModelState.Remove(nameof(model.StripePublishableKey));
            ModelState.Remove(nameof(model.CartTotal));
            ModelState.Remove(nameof(model.CartItems));
            ModelState.Remove(nameof(model.SavedAddresses));

            if (model.SelectedAddressId.HasValue)
            {
                ModelState.Remove(nameof(model.FirstName));
                ModelState.Remove(nameof(model.LastName));
                ModelState.Remove(nameof(model.Phone));
                ModelState.Remove(nameof(model.Address));
                ModelState.Remove(nameof(model.City));
                ModelState.Remove(nameof(model.PostalCode));
                ModelState.Remove(nameof(model.Country));
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                ModelState.Remove(nameof(model.Email));

                if (string.IsNullOrEmpty(model.Email))
                {
                    var user = await userManager.GetUserAsync(User);
                    model.Email = user?.Email ?? "";
                }
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                var cartItems = shopCartRepository.GetShoppingCartitems();
                var savedAddresses = addressRepository.GetUserAddresses(currentUserId);

                model.CartItems = cartItems.ToList();
                model.CartTotal = cartItems.Sum(i => i.Product.Price * i.Qty);
                model.SavedAddresses = savedAddresses;
                model.StripePublishableKey = configuration["Stripe:PublishableKey"] ?? "";

                TempData["Error"] = "Please fill in all required fields: " + string.Join(", ", errors);

                return View(model);
            }

            if (model.SelectedAddressId.HasValue)
            {
                var address = addressRepository.GetAddressById(model.SelectedAddressId.Value);
                if (address != null)
                {
                    model.FirstName = address.FirstName;
                    model.LastName = address.LastName;
                    model.Phone = address.Phone;
                    model.Address = address.StreetAddress;
                    model.City = address.City;
                    model.PostalCode = address.PostalCode;
                    model.Country = address.Country;
                }
            }
            else if (model.SaveAddress && !model.SelectedAddressId.HasValue)
            {
                var newAddress = new CoffeeShop.Models.Address
                {
                    UserID = currentUserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Phone = model.Phone,
                    StreetAddress = model.Address,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    Country = model.Country,
                    CreatedDate = DateTime.Now
                };

                try
                {
                    addressRepository.AddAddress(newAddress);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving address: {ex.Message}");
                }
            }

            PaymentResult paymentResult = null;
            if (model.PaymentMethod == "Stripe")
            {
                if (string.IsNullOrEmpty(model.StripeToken))
                {
                    ModelState.AddModelError("", "Payment token is missing. Please try again.");

                    var cartItems = shopCartRepository.GetShoppingCartitems();
                    var savedAddresses = addressRepository.GetUserAddresses(currentUserId);

                    model.CartItems = cartItems.ToList();
                    model.CartTotal = cartItems.Sum(i => i.Product.Price * i.Qty);
                    model.SavedAddresses = savedAddresses;
                    model.StripePublishableKey = configuration["Stripe:PublishableKey"] ?? "";

                    return View(model);
                }

                StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

                var options = new ChargeCreateOptions
                {
                    Amount = (long)(model.CartTotal * 100),
                    Currency = "usd",
                    Source = model.StripeToken,
                    Description = $"Order for {model.FirstName} {model.LastName}"
                };

                var service = new ChargeService();
                try
                {
                    var charge = await service.CreateAsync(options);

                    paymentResult = new PaymentResult
                    {
                        Success = charge.Status == "succeeded",
                        TransactionId = charge.Id,
                        Message = charge.Status
                    };
                }
                catch (StripeException e)
                {
                    paymentResult = new PaymentResult
                    {
                        Success = false,
                        Message = e.Message
                    };

                    ModelState.AddModelError("", "Payment failed: " + e.Message);

                    var cartItems = shopCartRepository.GetShoppingCartitems();
                    var savedAddresses = addressRepository.GetUserAddresses(currentUserId);

                    model.CartItems = cartItems.ToList();
                    model.CartTotal = cartItems.Sum(i => i.Product.Price * i.Qty);
                    model.SavedAddresses = savedAddresses;
                    model.StripePublishableKey = configuration["Stripe:PublishableKey"] ?? "";

                    return View(model);
                }
            }
            else
            {
                paymentResult = new PaymentResult
                {
                    Success = true,
                    Message = "Cash on Delivery"
                };
            }

            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) ||
                string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Phone) ||
                string.IsNullOrEmpty(model.Address) || string.IsNullOrEmpty(model.City) ||
                string.IsNullOrEmpty(model.Country))
            {
                ModelState.AddModelError("", "Missing required order information");

                var cartItems = shopCartRepository.GetShoppingCartitems();
                var savedAddresses = addressRepository.GetUserAddresses(currentUserId);

                model.CartItems = cartItems.ToList();
                model.CartTotal = cartItems.Sum(i => i.Product.Price * i.Qty);
                model.SavedAddresses = savedAddresses;
                model.StripePublishableKey = configuration["Stripe:PublishableKey"] ?? "";

                return View(model);
            }

            var order = new Order
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country,
                Notes = model.Notes,
                UserID = currentUserId,
                OrderPlaced = DateTime.Now
            };

            orderRepository.PlaceOrder(order);

            if (paymentResult != null && paymentResult.Success)
            {
                var payment = new Payment
                {
                    OrderID = order.OrderID,
                    PaymentMethod = model.PaymentMethod == "Stripe" ? "CreditCard" : "Cash",
                    Amount = model.CartTotal,
                    PaymentStatus = "Completed",
                    TransactionID = paymentResult.TransactionId,
                    PaymentDate = DateTime.Now
                };
                paymentRepository.CreatePayment(payment);
            }

            try
            {
                shopCartRepository.ClearCart();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing cart: {ex.Message}");
            }

            try
            {
                await emailService.SendOrderConfirmationEmailAsync(
                    order.Email,
                    $"{order.FirstName} {order.LastName}",
                    order.OrderID,
                    model.CartTotal
                );
            }
            catch (Exception emailEx)
            {
                Console.WriteLine($"Error sending confirmation email: {emailEx.Message}");
            }

            return RedirectToAction("CheckoutComplete", new { id = order.OrderID });
        }

        public IActionResult CheckoutComplete(int id)
        {
            var order = orderRepository.GetOrderById(id);

            if (order == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (order.UserID != userId)
            {
                return Forbid();
            }

            return View(order);
        }

        public IActionResult OrderDetails(int id)
        {
            var order = orderRepository.GetOrderById(id);

            if (order == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (order.UserID != userId)
            {
                return Forbid();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(int orderId)
        {
            var order = orderRepository.GetOrderById(orderId);

            if (order == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (order.UserID != userId)
            {
                return Forbid();
            }

            if (order.OrderStatus == "Pending")
            {
                orderRepository.UpdateOrderStatus(orderId, "Cancelled");
                TempData["Success"] = "Porosia u anulua me sukses.";
            }
            else
            {
                TempData["Error"] = "Kjo porosi nuk mund të anulohet.";
            }

            return RedirectToAction("OrderDetails", new { id = orderId });
        }
    }
}