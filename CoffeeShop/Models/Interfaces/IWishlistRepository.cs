namespace CoffeeShop.Models.Interfaces
{
    public interface IWishlistRepository
    {
        // Shto produkt në wishlist
        void AddToWishlist(string userId, int productId);

        // Largo produkt nga wishlist
        void RemoveFromWishlist(string userId, int productId);

        // Merr wishlist-in e përdoruesit
        IEnumerable<Wishlist> GetUserWishlist(string userId);

        // Kontrollo nëse produkti është në wishlist
        bool IsInWishlist(string userId, int productId);

        // Pastro wishlist-in
        void ClearWishlist(string userId);

        // Merr numrin e produkteve në wishlist
        int GetWishlistCount(string userId);
    }
}