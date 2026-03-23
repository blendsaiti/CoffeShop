namespace CoffeeShop.Models.Interfaces
{
    public interface IOrderRepository
    {
        // Vendos një porosi
        void PlaceOrder(Order order);

        // Merr një porosi sipas ID-së
        Order? GetOrderById(int orderId);

        // Merr të gjitha porosite
        IEnumerable<Order> GetAllOrders();

        // Merr porosite e një përdoruesi
        IEnumerable<Order> GetOrdersByUser(string userId);

        // Merr porosite më të fundit
        IEnumerable<Order> GetRecentOrders(int count);

        // Përditëso statusin e porosisë
        void UpdateOrderStatus(int orderId, string status);

        // Merr totalin e të ardhurave
        decimal GetTotalRevenue();

        // Merr totalin e shpenzuar nga një përdorues
        decimal GetTotalSpentByUser(string userId);

        // Merr shitjet ditore
        Dictionary<DateTime, decimal> GetDailySales(int days);

        // Merr të ardhurat mujore
        Dictionary<string, decimal> GetMonthlyRevenue(int months);

        // Merr statistikat e porosive
        OrderStatistics GetOrderStatistics();
    }

    public class OrderStatistics
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}