using HairSalonDemo.Data;
using HairSalonDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HairSalonDemo.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
            return View(appointments);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null) return NotFound();
            return View(appointment);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new Appointment());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,CustomerId,StaffId,ServiceId,AppointmentDate,AppointmentTime,Status")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(appointment.CustomerId, appointment.StaffId, appointment.ServiceId);
            return View(appointment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            await PopulateDropdowns(appointment.CustomerId, appointment.StaffId, appointment.ServiceId);
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,CustomerId,StaffId,ServiceId,AppointmentDate,AppointmentTime,Status")] Appointment appointment)
        {
            if (id != appointment.AppointmentId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Appointments.Any(a => a.AppointmentId == appointment.AppointmentId)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(appointment.CustomerId, appointment.StaffId, appointment.ServiceId);
            return View(appointment);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null) return NotFound();
            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(int? customerId = null, int? staffId = null, int? serviceId = null)
        {
            ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.FullName).ToListAsync(), "CustomerId", "FullName", customerId);
            ViewData["StaffId"] = new SelectList(await _context.Staff.Where(s => s.IsActive).OrderBy(s => s.FullName).ToListAsync(), "StaffId", "FullName", staffId);
            ViewData["ServiceId"] = new SelectList(await _context.Services.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync(), "ServiceId", "Name", serviceId);
        }
    }
}
