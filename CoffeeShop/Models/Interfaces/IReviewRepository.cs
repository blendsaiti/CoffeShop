namespace CoffeeShop.Models.Interfaces
{
    public interface IReviewRepository
    {
        // Merr të gjitha review-t
        IEnumerable<Review> GetAllReviews();

        // Merr review-t për një produkt specifik
        IEnumerable<Review> GetReviewsByProduct(int productId);

        // Merr review-t e një përdoruesi
        IEnumerable<Review> GetReviewsByUser(string userId);

        // Merr një review specifik
        Review? GetReviewById(int reviewId);

        // Shton një review të ri
        Review AddReview(Review review);

        // Përditëson një review
        Review UpdateReview(Review review);

        // Fshin një review
        void DeleteReview(int reviewId);

        // Aprovo një review
        void ApproveReview(int reviewId);

        // Merr review-t e pa-aprovuara
        IEnumerable<Review> GetPendingReviews();

        // Kontrollon nëse përdoruesi ka bërë review për produktin
        bool HasUserReviewedProduct(string userId, int productId);
    }
}