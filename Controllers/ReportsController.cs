using HairSalonDemo.Data;
using HairSalonDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HairSalonDemo.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var sales = _context.Sales.AsQueryable();

            var topStaff = await _context.Sales
                .Include(s => s.Staff)
                .Where(s => s.Staff != null)
                .GroupBy(s => s.Staff!.FullName)
                .Select(g => new { StaffName = g.Key, Total = g.Sum(x => x.TotalAmount) })
                .OrderByDescending(x => x.Total)
                .FirstOrDefaultAsync();

            var mostPopularService = await _context.SaleItems
                .Where(i => i.ItemType == "Service")
                .GroupBy(i => i.Description)
                .Select(g => new { ServiceName = g.Key, Count = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            var staffCommissions = await _context.Staff
                .Select(staff => new StaffCommissionRow
                {
                    StaffName = staff.FullName,
                    CommissionRate = staff.CommissionRate,
                    ServiceSalesTotal = _context.Sales
                        .Where(s => s.StaffId == staff.StaffId)
                        .SelectMany(s => s.SaleItems!)
                        .Where(i => i.ItemType == "Service")
                        .Select(i => (decimal?)i.LineTotal)
                        .Sum() ?? 0
                })
                .ToListAsync();

            foreach (var row in staffCommissions)
            {
                row.CommissionAmount = row.ServiceSalesTotal * (row.CommissionRate / 100m);
            }

            var model = new ReportsViewModel
            {
                DailySales = await sales.Where(s => s.SaleDate.Date == today).SumAsync(s => (decimal?)s.TotalAmount) ?? 0,
                WeeklySales = await sales.Where(s => s.SaleDate.Date >= weekStart).SumAsync(s => (decimal?)s.TotalAmount) ?? 0,
                MonthlySales = await sales.Where(s => s.SaleDate.Date >= monthStart).SumAsync(s => (decimal?)s.TotalAmount) ?? 0,
                TopStaffMember = topStaff?.StaffName ?? "N/A",
                TopStaffSales = topStaff?.Total ?? 0,
                MostPopularService = mostPopularService?.ServiceName ?? "N/A",
                MostPopularServiceCount = mostPopularService?.Count ?? 0,
                StaffCommissions = staffCommissions.OrderByDescending(s => s.CommissionAmount).ToList()
            };

            return View(model);
        }
    }
}
