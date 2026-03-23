using CoffeeShop.Models;
using CoffeeShop.Models.Interfaces;
using CoffeeShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoffeeShop.Controllers
{
    [Authorize] 
    public class UserDashboardController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly IReviewRepository reviewRepository;
        private readonly IProductRepository productRepository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IWishlistRepository wishlistRepository;
        private readonly IAddressRepository addressRepository;

        public UserDashboardController(
    IOrderRepository orderRepository,
    IReviewRepository reviewRepository,
    IProductRepository productRepository,
    IWishlistRepository wishlistRepository,
    IAddressRepository addressRepository,
    UserManager<IdentityUser> userManager)
        {
            this.orderRepository = orderRepository;
            this.reviewRepository = reviewRepository;
            this.productRepository = productRepository;
            this.wishlistRepository = wishlistRepository;
            this.addressRepository = addressRepository;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            var viewModel = new UserDashboardViewModel
            {
                User = user,
                RecentOrders = orderRepository.GetOrdersByUser(userId).Take(5),
                TotalOrders = orderRepository.GetOrdersByUser(userId).Count(),
                TotalSpent = orderRepository.GetTotalSpentByUser(userId),
                RecentReviews = reviewRepository.GetReviewsByUser(userId).Take(5),
                WishlistCount = wishlistRepository.GetWishlistCount(userId),
                ReviewCount = reviewRepository.GetReviewsByUser(userId).Count()
            };

            return View(viewModel);
        }

        public IActionResult OrderHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = orderRepository.GetOrdersByUser(userId);
            return View(orders);
        }
        public IActionResult OrderDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = orderRepository.GetOrderById(id);

            if (order == null || order.UserID != userId)
            {
                return NotFound();
            }

            return View(order);
        }

        public IActionResult MyReviews()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reviews = reviewRepository.GetReviewsByUser(userId);
            return View(reviews);
        }

        [HttpGet]
        public IActionResult AddReview(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = productRepository.GetProductDetail(productId);

            if (product == null)
            {
                return NotFound();
            }

            if (reviewRepository.HasUserReviewedProduct(userId, productId))
            {
                TempData["Error"] = "Ju keni bërë tashmë një review për këtë produkt.";
                return RedirectToAction("Detail", "Products", new { id = productId });
            }

            var viewModel = new AddReviewViewModel
            {
                ProductID = productId,
                ProductName = product.Name
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(AddReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            var review = new Review
            {
                ProductID = model.ProductID,
                UserID = userId,
                UserName = user.UserName,
                Rating = model.Rating,
                Comment = model.Comment,
                ReviewDate = DateTime.Now,
                IsApproved = false
            };

            reviewRepository.AddReview(review);

            TempData["Success"] = "Review-i juaj është dërguar dhe pritet aprovimi.";
            return RedirectToAction("Detail", "Products", new { id = model.ProductID });
        }

        [HttpGet]
        public IActionResult EditReview(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = reviewRepository.GetReviewById(id);

            if (review == null || review.UserID != userId)
            {
                return NotFound();
            }

            var viewModel = new EditReviewViewModel
            {
                ReviewID = review.ReviewID,
                ProductID = review.ProductID,
                ProductName = review.Product?.Name,
                Rating = review.Rating,
                Comment = review.Comment
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditReview(EditReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = reviewRepository.GetReviewById(model.ReviewID);

            if (review == null || review.UserID != userId)
            {
                return NotFound();
            }

            review.Rating = model.Rating;
            review.Comment = model.Comment;

            reviewRepository.UpdateReview(review);

            TempData["Success"] = "Review-i juaj u përditësua me sukses.";
            return RedirectToAction("MyReviews");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteReview(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = reviewRepository.GetReviewById(id);

            if (review == null || review.UserID != userId)
            {
                return NotFound();
            }

            reviewRepository.DeleteReview(id);

            TempData["Success"] = "Review-i u fshi me sukses.";
            return RedirectToAction("MyReviews");
        }

        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            return View(user);
        }

        public IActionResult Wishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlist = wishlistRepository.GetUserWishlist(userId);
            return View(wishlist);
        }

        public IActionResult AddToWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                wishlistRepository.AddToWishlist(userId, productId);
                TempData["Success"] = "Product added to wishlist!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error adding to wishlist: {ex.Message}";
            }

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Wishlist");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                wishlistRepository.RemoveFromWishlist(userId, productId);
                TempData["Success"] = "Product removed from wishlist!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error removing from wishlist: {ex.Message}";
            }

            return RedirectToAction("Wishlist");
        }


        public IActionResult Addresses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var addresses = addressRepository.GetUserAddresses(userId);
            return View(addresses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAddress(Address address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            address.UserID = userId;
            address.CreatedDate = DateTime.Now;

            ModelState.Remove(nameof(address.UserID));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                TempData["Error"] = $"Please fix: {string.Join(", ", errors)}";
                return RedirectToAction("Addresses");
            }

            try
            {
                addressRepository.AddAddress(address);
                TempData["Success"] = "Address added successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Addresses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAddress(Address address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            address.UserID = userId;
            address.CreatedDate = DateTime.Now;

            ModelState.Remove(nameof(address.UserID));

            if (ModelState.IsValid)
            {
                var existingAddress = addressRepository.GetAddressById(address.AddressID);

                if (existingAddress != null && existingAddress.UserID == userId)
                {
                    address.UserID = userId; 
                    addressRepository.UpdateAddress(address);
                    TempData["Success"] = "Address updated successfully!";
                }
                else
                {
                    TempData["Error"] = "Address not found.";
                }
            }
            else
            {
                TempData["Error"] = "Please fill all required fields.";
            }
            return RedirectToAction("Addresses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAddress(int addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var address = addressRepository.GetAddressById(addressId);

            if (address != null && address.UserID == userId)
            {
                addressRepository.DeleteAddress(addressId);
                TempData["Success"] = "Address deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Address not found.";
            }

            return RedirectToAction("Addresses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetDefaultAddress(int addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var address = addressRepository.GetAddressById(addressId);

            if (address != null && address.UserID == userId)
            {
                addressRepository.SetDefaultAddress(userId, addressId);
                TempData["Success"] = "Default address updated!";
            }
            else
            {
                TempData["Error"] = "Address not found.";
            }

            return RedirectToAction("Addresses");
        }
    }
}