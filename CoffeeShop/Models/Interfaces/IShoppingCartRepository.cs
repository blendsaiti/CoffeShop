namespace CoffeeShop.Models.Interfaces // Deklarimi i namespace-it për interfejsin IShoppingCartRepository
{

    public interface IShoppingCartRepository
    {
        void AddToCart(Product product); // Metoda për të shtuar një produkt në shportën e blerjeve
        int RemoveFromCart(Product product); // Metoda për të larguar një produkt nga shporta e blerjeve
        List<ShoppingCartitem> GetShoppingCartitems(); // Metoda për të marrë elementet e shportës së blerjeve
        void ClearCart(); // Metoda për të pastruar shportën e blerjeve
        decimal GetShoppingCartTotal();  // Metoda për të marrë totalin e shportës së blerjeve
        public List<ShoppingCartitem>? ShoppingCartitems { get; set; } // Lista e elementeve të shportës së blerjeve
    }

}

