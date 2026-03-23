using CoffeeShop.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int PendingReviews { get; set; }
        public decimal TotalRevenue { get; set; }
        public IEnumerable<Order>? RecentOrders { get; set; }
        public IEnumerable<Product>? LowStockProducts { get; set; }
        public int PendingPayments { get; set; }
    }

    public class SalesReportViewModel
    {
        public Dictionary<DateTime, decimal>? DailySales { get; set; }
        public IEnumerable<ProductSalesInfo>? TopProducts { get; set; }
        public Dictionary<string, decimal>? RevenueByMonth { get; set; }
    }

    public class ProductSalesInfo
    {
        public Product? Product { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class UserDashboardViewModel
    {
        public IdentityUser? User { get; set; }
        public IEnumerable<Order>? RecentOrders { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public IEnumerable<Review>? RecentReviews { get; set; }
        public int WishlistCount { get; set; }
        public int ReviewCount { get; set; }
    }

    public class AddReviewViewModel
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot be longer than 1000 characters")]
        public string? Comment { get; set; }
    }

    public class EditReviewViewModel
    {
        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot be longer than 1000 characters")]
        public string? Comment { get; set; }
    }

    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Phone number is not valid")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public string? PaymentMethod { get; set; }

        public IEnumerable<Address>? SavedAddresses { get; set; }
        public int? SelectedAddressId { get; set; }
        public bool SaveAddress { get; set; }

        public string? StripeToken { get; set; }
        public string? StripePublishableKey { get; set; }

        // Display properties
        public decimal CartTotal { get; set; }
        public List<ShoppingCartitem>? CartItems { get; set; }
    }

    public class OrderDetailsViewModel
    {
        public Order? Order { get; set; }
        public Payment? Payment { get; set; }
        public bool CanCancel { get; set; }
    }

    public class ProductListViewModel
    {
        public IEnumerable<Product>? Products { get; set; }
        public IEnumerable<Category>? Categories { get; set; }

        public string? SearchTerm { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string? SortBy { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalProducts { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int StartProduct => (CurrentPage - 1) * PageSize + 1;
        public int EndProduct => Math.Min(CurrentPage * PageSize, TotalProducts);
    }

    public class ProductDetailViewModel
    {
        public Product? Product { get; set; }
        public IEnumerable<Review>? ApprovedReviews { get; set; }
        public bool UserHasReviewed { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}