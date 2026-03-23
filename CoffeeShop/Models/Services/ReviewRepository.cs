using CoffeeShop.Data;
using CoffeeShop.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Models.Services
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly CoffeeShopDbContext dbContext;

        public ReviewRepository(CoffeeShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<Review> GetAllReviews()
        {
            return dbContext.Reviews
                .Include(r => r.Product)
                .OrderByDescending(r => r.ReviewDate)
                .ToList();
        }

        public IEnumerable<Review> GetReviewsByProduct(int productId)
        {
            return dbContext.Reviews
                .Where(r => r.ProductID == productId && r.IsApproved)
                .OrderByDescending(r => r.ReviewDate)
                .ToList();
        }

        public IEnumerable<Review> GetReviewsByUser(string userId)
        {
            return dbContext.Reviews
                .Where(r => r.UserID == userId)
                .Include(r => r.Product)
                .OrderByDescending(r => r.ReviewDate)
                .ToList();
        }

        public Review? GetReviewById(int reviewId)
        {
            return dbContext.Reviews
                .Include(r => r.Product)
                .FirstOrDefault(r => r.ReviewID == reviewId);
        }

        public Review AddReview(Review review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            review.ReviewDate = DateTime.Now;
            review.IsApproved = false; // Review-t duhet të aprovohen nga admin-i

            dbContext.Reviews.Add(review);
            dbContext.SaveChanges();

            return review;
        }

        public Review UpdateReview(Review review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            var existingReview = dbContext.Reviews.FirstOrDefault(r => r.ReviewID == review.ReviewID);

            if (existingReview != null)
            {
                existingReview.Rating = review.Rating;
                existingReview.Comment = review.Comment;
                existingReview.ReviewDate = DateTime.Now;

                dbContext.SaveChanges();
            }

            return existingReview;
        }

        public void DeleteReview(int reviewId)
        {
            var review = dbContext.Reviews.FirstOrDefault(r => r.ReviewID == reviewId);

            if (review != null)
            {
                dbContext.Reviews.Remove(review);
                dbContext.SaveChanges();
            }
        }

        public void ApproveReview(int reviewId)
        {
            var review = dbContext.Reviews.FirstOrDefault(r => r.ReviewID == reviewId);

            if (review != null)
            {
                review.IsApproved = true;
                dbContext.SaveChanges();
            }
        }

        public IEnumerable<Review> GetPendingReviews()
        {
            return dbContext.Reviews
                .Where(r => !r.IsApproved)
                .Include(r => r.Product)
                .OrderByDescending(r => r.ReviewDate)
                .ToList();
        }

        public bool HasUserReviewedProduct(string userId, int productId)
        {
            return dbContext.Reviews
                .Any(r => r.UserID == userId && r.ProductID == productId);
        }
    }
}