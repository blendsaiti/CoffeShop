using CoffeeShop.ViewModels;

namespace CoffeeShop.Models.Interfaces
{
    public interface IProductRepository
    {
        // Merr të gjitha produktet
        IEnumerable<Product> GetAllProducts();

        // Merr produktet e trendeve
        IEnumerable<Product> GetTrendingProducts();

        // Merr detajet e një produkti
        Product? GetProductDetail(int id);

        // Përditëso një produkt
        Product? UpdateProduct(Product updatedProduct);

        // Fshi një produkt
        void DeleteProduct(int id);

        // Shto një produkt të ri
        Product AddProduct(Product newProduct);

        // Merr produktet sipas kategorisë
        IEnumerable<Product> GetProductsByCategory(int categoryId);

        // Kërkimi i produkteve
        IEnumerable<Product> SearchProducts(string searchTerm);

        // Merr produktet me stok të ulët
        IEnumerable<Product> GetLowStockProducts();

        // Përditëso stokun e produktit
        void UpdateStock(int productId, int quantity);

        // Merr produktet më të shitura
        IEnumerable<ProductSalesInfo> GetTopSellingProducts(int count);

        // Merr produktet e disponueshme
        IEnumerable<Product> GetAvailableProducts();

        // Kontrollo disponibilitetin e produktit
        bool IsProductAvailable(int productId, int quantity);
    }
}