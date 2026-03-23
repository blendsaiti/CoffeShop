namespace CoffeeShop.Models
{
    public class ShoppingCartitem
    {
        public int ID { get; set; } //Kjo është një property e tipit të tërë (integer) për identifikuesin e elementit të shportës së blerjeve.
        public Product Product { get; set; } // Kjo është një property e tipit "Product", e cila përmban vetë produktin që është shtuar në shportën e blerjeve.
                                             // Kjo është një marrëdhënie ndërmjet klasave, ku një element i shportës së blerjeve ka një produkt.
        public int Qty { get; set; } //Kjo është një property e tipit të tërë për sasine (sasi) e produktit që është shtuar në shportën e blerjeve.
        public string ShoppingCartID { get; set; } //: Kjo është një property e tipit string për identifikuesin e shportës së blerjeve.
                                                   //Çdo element i shportës së blerjeve është i lidhur me një shportë të caktuar përmes kësaj property.
    }
}
