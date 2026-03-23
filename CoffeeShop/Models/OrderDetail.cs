namespace CoffeeShop.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; } //Id e porosisë
        public int ProductID { get; set; } //Id e produktit që është përfshirë në këtë detaj të porosisë.
        public Product? Product { get; set; }  //Produkti që është përfshirë në këtë detaj të porosisë.
        public int OrderID { get; set; } //Id e porosisë për të cilën është ky detaj.
        public Order? Order { get; set; } //Porosia për të cilën është ky detaj.
        public int Quantity { get; set; } //Sasia e produktit që është porositur në këtë detaj të porosisë.
        public decimal Price { get; set; } // Çmimi për njësi i produktit në këtë detaj të porosisë.
    }
}
