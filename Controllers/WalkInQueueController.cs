using HairSalonDemo.Data;
using HairSalonDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HairSalonDemo.Controllers
{
    public class WalkInQueueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WalkInQueueController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.WalkInQueueItems
                .Include(w => w.Staff)
                .OrderBy(w => w.Status)
                .ThenBy(w => w.CreatedAt)
                .ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["StaffId"] = new SelectList(await _context.Staff.Where(s => s.IsActive).OrderBy(s => s.FullName).ToListAsync(), "StaffId", "FullName");
            return View(new WalkInQueueItem());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WalkInQueueItemId,CustomerName,StaffId,Status,CreatedAt")] WalkInQueueItem item)
        {
            if (ModelState.IsValid)
            {
                if (item.CreatedAt == default)
                {
                    item.CreatedAt = DateTime.Now;
                }
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["StaffId"] = new SelectList(await _context.Staff.Where(s => s.IsActive).OrderBy(s => s.FullName).ToListAsync(), "StaffId", "FullName", item.StaffId);
            return View(item);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.WalkInQueueItems.FindAsync(id);
            if (item == null) return NotFound();

            ViewData["StaffId"] = new SelectList(await _context.Staff.Where(s => s.IsActive).OrderBy(s => s.FullName).ToListAsync(), "StaffId", "FullName", item.StaffId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WalkInQueueItemId,CustomerName,StaffId,Status,CreatedAt")] WalkInQueueItem item)
        {
            if (id != item.WalkInQueueItemId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.WalkInQueueItems.Any(w => w.WalkInQueueItemId == item.WalkInQueueItemId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["StaffId"] = new SelectList(await _context.Staff.Where(s => s.IsActive).OrderBy(s => s.FullName).ToListAsync(), "StaffId", "FullName", item.StaffId);
            return View(item);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.WalkInQueueItems.Include(w => w.Staff).FirstOrDefaultAsync(w => w.WalkInQueueItemId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.WalkInQueueItems.FindAsync(id);
            if (item != null)
            {
                _context.WalkInQueueItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
