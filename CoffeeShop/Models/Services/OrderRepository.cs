using CoffeeShop.Data;
using CoffeeShop.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Models.Services
{
    public class OrderRepository : IOrderRepository
    {
        private CoffeeShopDbContext dbContext;
        private IShoppingCartRepository shopCartRepository;

        public OrderRepository(CoffeeShopDbContext dbContext, IShoppingCartRepository shopCartRepository)
        {
            this.dbContext = dbContext;
            this.shopCartRepository = shopCartRepository;
        }

        public void PlaceOrder(Order order)
        {
            var shoppingcartitems = shopCartRepository.GetShoppingCartitems();
            order.OrderDetails = new List<OrderDetail>();

            foreach (var item in shoppingcartitems)
            {
                var orderDetail = new OrderDetail
                {
                    Quantity = item.Qty,
                    ProductID = item.Product.ProductID,
                    Price = item.Product.Price
                };
                order.OrderDetails.Add(orderDetail);

                // Përditëso stokun e produktit
                var product = dbContext.Products.Find(item.Product.ProductID);
                if (product != null)
                {
                    product.StockQuantity -= item.Qty;
                }
            }

            order.OrderPlaced = DateTime.Now;
            order.OrderTotal = shopCartRepository.GetShoppingCartTotal();
            order.OrderStatus = "Pending"; // Statusi fillestare i porosisë

            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
        }

        public Order? GetOrderById(int orderId)
        {
            return dbContext.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Payment)
                .FirstOrDefault(o => o.OrderID == orderId);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return dbContext.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderPlaced)
                .ToList();
        }

        public IEnumerable<Order> GetOrdersByUser(string userId)
        {
            return dbContext.Orders
                .Where(o => o.UserID == userId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderPlaced)
                .ToList();
        }

        public IEnumerable<Order> GetRecentOrders(int count)
        {
            return dbContext.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderPlaced)
                .Take(count)
                .ToList();
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            var order = dbContext.Orders.Find(orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                dbContext.SaveChanges();
            }
        }

        public decimal GetTotalRevenue()
        {
            return dbContext.Orders
                .Where(o => o.OrderStatus == "Completed" || o.OrderStatus == "Shipped")
                .Sum(o => o.OrderTotal);
        }

        public decimal GetTotalSpentByUser(string userId)
        {
            return dbContext.Orders
                .Where(o => o.UserID == userId)
                .Sum(o => o.OrderTotal);
        }

        public Dictionary<DateTime, decimal> GetDailySales(int days)
        {
            var startDate = DateTime.Now.AddDays(-days);

            var sales = dbContext.Orders
                .Where(o => o.OrderPlaced >= startDate)
                .GroupBy(o => o.OrderPlaced.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Total = g.Sum(o => o.OrderTotal)
                })
                .ToDictionary(x => x.Date, x => x.Total);

            return sales;
        }

        public Dictionary<string, decimal> GetMonthlyRevenue(int months)
        {
            var startDate = DateTime.Now.AddMonths(-months);

            var revenue = dbContext.Orders
                .Where(o => o.OrderPlaced >= startDate)
                .GroupBy(o => new { o.OrderPlaced.Year, o.OrderPlaced.Month })
                .Select(g => new
                {
                    Month = g.Key.Year + "-" + g.Key.Month.ToString("D2"),
                    Total = g.Sum(o => o.OrderTotal)
                })
                .ToDictionary(x => x.Month, x => x.Total);

            return revenue;
        }

        public OrderStatistics GetOrderStatistics()
        {
            var orders = dbContext.Orders.ToList();

            return new OrderStatistics
            {
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.OrderStatus == "Pending"),
                CompletedOrders = orders.Count(o => o.OrderStatus == "Completed"),
                CancelledOrders = orders.Count(o => o.OrderStatus == "Cancelled"),
                TotalRevenue = orders.Where(o => o.OrderStatus == "Completed" || o.OrderStatus == "Shipped").Sum(o => o.OrderTotal),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.OrderTotal) : 0
            };
        }
    }
}