using HairSalonDemo.Data;
using HairSalonDemo.Models;
using HairSalonDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HairSalonDemo.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Staff)
                .Include(s => s.SaleItems)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            return View(sales);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Staff)
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.SaleId == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        public async Task<IActionResult> Create()
        {
            var model = new CreateSaleViewModel();
            await PopulateSaleDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSaleViewModel model)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == model.ServiceId && s.IsActive);
            var product = model.ProductId.HasValue
                ? await _context.Products.FirstOrDefaultAsync(p => p.ProductId == model.ProductId.Value && p.IsActive)
                : null;

            if (service == null)
            {
                ModelState.AddModelError(nameof(model.ServiceId), "Please select a valid service.");
            }

            if (model.ProductId.HasValue && product == null)
            {
                ModelState.AddModelError(nameof(model.ProductId), "Please select a valid product.");
            }

            if (product != null && model.ProductQuantity > product.StockQuantity)
            {
                ModelState.AddModelError(nameof(model.ProductQuantity), $"Only {product.StockQuantity} unit(s) available in stock.");
            }

            if (!ModelState.IsValid || service == null)
            {
                await PopulateSaleDropdowns(model);
                return View(model);
            }

            var subtotal = service.Price;
            if (product != null)
            {
                subtotal += product.SellingPrice * model.ProductQuantity;
            }

            var total = subtotal - model.DiscountAmount;
            if (total < 0)
            {
                total = 0;
            }

            var sale = new Sale
            {
                CustomerId = model.CustomerId,
                StaffId = model.StaffId,
                SaleDate = DateTime.Now,
                Subtotal = subtotal,
                DiscountAmount = model.DiscountAmount,
                TotalAmount = total,
                PaymentMethod = model.PaymentMethod,
                SaleItems = new List<SaleItem>()
            };

            sale.SaleItems.Add(new SaleItem
            {
                ItemType = "Service",
                ItemId = service.ServiceId,
                Description = service.Name,
                Quantity = 1,
                UnitPrice = service.Price,
                LineTotal = service.Price
            });

            if (product != null)
            {
                var productLineTotal = product.SellingPrice * model.ProductQuantity;
                sale.SaleItems.Add(new SaleItem
                {
                    ItemType = "Product",
                    ItemId = product.ProductId,
                    Description = product.Name,
                    Quantity = model.ProductQuantity,
                    UnitPrice = product.SellingPrice,
                    LineTotal = productLineTotal
                });

                product.StockQuantity -= model.ProductQuantity;
                if (product.StockQuantity <= product.ReorderLevel)
                {
                    TempData["LowStockWarning"] = $"Low stock warning: {product.Name} is now at {product.StockQuantity} unit(s).";
                }
            }

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = sale.SaleId });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.FullName).ToListAsync(), "CustomerId", "FullName", sale.CustomerId);
            ViewData["StaffId"] = new SelectList(await _context.Staff.OrderBy(s => s.FullName).ToListAsync(), "StaffId", "FullName", sale.StaffId);
            return View(sale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SaleId,CustomerId,StaffId,SaleDate,Subtotal,DiscountAmount,TotalAmount,PaymentMethod")] Sale sale)
        {
            if (id != sale.SaleId)
            {
                return NotFound();
            }

            if (sale.TotalAmount < 0)
            {
                ModelState.AddModelError(nameof(sale.TotalAmount), "Total cannot be negative.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.FullName).ToListAsync(), "CustomerId", "FullName", sale.CustomerId);
                ViewData["StaffId"] = new SelectList(await _context.Staff.OrderBy(s => s.FullName).ToListAsync(), "StaffId", "FullName", sale.StaffId);
                return View(sale);
            }

            try
            {
                _context.Update(sale);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleExists(sale.SaleId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Staff)
                .FirstOrDefaultAsync(m => m.SaleId == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.SaleId == id);

            if (sale != null)
            {
                if (sale.SaleItems != null && sale.SaleItems.Any())
                {
                    _context.SaleItems.RemoveRange(sale.SaleItems);
                }
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.SaleId == id);
        }

        private async Task PopulateSaleDropdowns(CreateSaleViewModel model)
        {
            model.Customers = await _context.Customers
                .OrderBy(c => c.FullName)
                .Select(c => new SelectListItem { Value = c.CustomerId.ToString(), Text = c.FullName })
                .ToListAsync();

            model.StaffMembers = await _context.Staff
                .Where(s => s.IsActive)
                .OrderBy(s => s.FullName)
                .Select(s => new SelectListItem { Value = s.StaffId.ToString(), Text = s.FullName })
                .ToListAsync();

            model.Services = await _context.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem { Value = s.ServiceId.ToString(), Text = s.Name + " - " + s.Price.ToString("C") })
                .ToListAsync();

            model.Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem { Value = p.ProductId.ToString(), Text = p.Name + " - " + p.SellingPrice.ToString("C") + " (Stock: " + p.StockQuantity + ")" })
                .ToListAsync();
        }
    }
}
