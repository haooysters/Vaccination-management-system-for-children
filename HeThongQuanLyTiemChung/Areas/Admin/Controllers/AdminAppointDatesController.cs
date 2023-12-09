using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyTiemChung.Models;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAppointDatesController : Controller
    {
        private readonly db_VaccineContext _context;

        public AdminAppointDatesController(db_VaccineContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminAppointDates
        public async Task<IActionResult> Index()
        {
            var db_VaccineContext = _context.AppointDates.Include(a => a.Customer).Include(a => a.Injection);
            return View(await db_VaccineContext.ToListAsync());
        }

        // GET: Admin/AdminAppointDates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointDate = await _context.AppointDates
                .Include(a => a.Customer)
                .Include(a => a.Injection)
                .FirstOrDefaultAsync(m => m.AppointDateId == id);
            if (appointDate == null)
            {
                return NotFound();
            }

            return View(appointDate);
        }

        // GET: Admin/AdminAppointDates/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["InjectionId"] = new SelectList(_context.Injections, "InjectionId", "InjectionId");
            return View();
        }

        // POST: Admin/AdminAppointDates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointDateId,InjectionId,CustomerId,AppointmentDate")] AppointDate appointDate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointDate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", appointDate.CustomerId);
            ViewData["InjectionId"] = new SelectList(_context.Injections, "InjectionId", "InjectionId", appointDate.InjectionId);
            return View(appointDate);
        }

        // GET: Admin/AdminAppointDates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointDate = await _context.AppointDates.FindAsync(id);
            if (appointDate == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", appointDate.CustomerId);
            ViewData["InjectionId"] = new SelectList(_context.Injections, "InjectionId", "InjectionId", appointDate.InjectionId);
            return View(appointDate);
        }

        // POST: Admin/AdminAppointDates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointDateId,InjectionId,CustomerId,AppointmentDate")] AppointDate appointDate)
        {
            if (id != appointDate.AppointDateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointDate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointDateExists(appointDate.AppointDateId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", appointDate.CustomerId);
            ViewData["InjectionId"] = new SelectList(_context.Injections, "InjectionId", "InjectionId", appointDate.InjectionId);
            return View(appointDate);
        }

        // GET: Admin/AdminAppointDates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointDate = await _context.AppointDates
                .Include(a => a.Customer)
                .Include(a => a.Injection)
                .FirstOrDefaultAsync(m => m.AppointDateId == id);
            if (appointDate == null)
            {
                return NotFound();
            }

            return View(appointDate);
        }

        // POST: Admin/AdminAppointDates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointDate = await _context.AppointDates.FindAsync(id);
            _context.AppointDates.Remove(appointDate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointDateExists(int id)
        {
            return _context.AppointDates.Any(e => e.AppointDateId == id);
        }
    }
}
