using HairSalonDemo.Models;

namespace HairSalonDemo.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TodaySales { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalServices { get; set; }
        public int LowStockCount { get; set; }
        public List<Sale> RecentSales { get; set; } = new();
        public List<Product> LowStockProducts { get; set; } = new();
    }
}
