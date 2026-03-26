using HairSalonDemo.Data;
using HairSalonDemo.Models;
using HairSalonDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HairSalonDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var model = new DashboardViewModel
            {
                TodaySales = await _context.Sales
                    .Where(s => s.SaleDate >= today && s.SaleDate < tomorrow)
                    .SumAsync(s => (decimal?)s.TotalAmount) ?? 0m,

                TotalCustomers = await _context.Customers.CountAsync(),

                TotalServices = await _context.Services.CountAsync(),

                LowStockCount = await _context.Products
                    .CountAsync(p => p.StockQuantity <= p.ReorderLevel),

                RecentSales = await _context.Sales
                    .Include(s => s.Customer)
                    .Include(s => s.Staff)
                    .OrderByDescending(s => s.SaleDate)
                    .Take(5)
                    .ToListAsync(),

                LowStockProducts = await _context.Products
                    .Where(p => p.StockQuantity <= p.ReorderLevel)
                    .OrderBy(p => p.StockQuantity)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}