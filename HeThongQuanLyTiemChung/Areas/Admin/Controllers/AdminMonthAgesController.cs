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
    public class AdminMonthAgesController : Controller
    {
        private readonly db_VaccineContext _context;

        public AdminMonthAgesController(db_VaccineContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminMonthAges
        public async Task<IActionResult> Index()
        {
            return View(await _context.MonthAges.ToListAsync());
        }

        // GET: Admin/AdminMonthAges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monthAge = await _context.MonthAges
                .FirstOrDefaultAsync(m => m.MonthAgeId == id);
            if (monthAge == null)
            {
                return NotFound();
            }

            return View(monthAge);
        }

        // GET: Admin/AdminMonthAges/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminMonthAges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MonthAgeId,MonthAgeName")] MonthAge monthAge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monthAge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(monthAge);
        }

        // GET: Admin/AdminMonthAges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monthAge = await _context.MonthAges.FindAsync(id);
            if (monthAge == null)
            {
                return NotFound();
            }
            return View(monthAge);
        }

        // POST: Admin/AdminMonthAges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MonthAgeId,MonthAgeName")] MonthAge monthAge)
        {
            if (id != monthAge.MonthAgeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monthAge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonthAgeExists(monthAge.MonthAgeId))
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
            return View(monthAge);
        }

        // GET: Admin/AdminMonthAges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monthAge = await _context.MonthAges
                .FirstOrDefaultAsync(m => m.MonthAgeId == id);
            if (monthAge == null)
            {
                return NotFound();
            }

            return View(monthAge);
        }

        // POST: Admin/AdminMonthAges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monthAge = await _context.MonthAges.FindAsync(id);
            _context.MonthAges.Remove(monthAge);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonthAgeExists(int id)
        {
            return _context.MonthAges.Any(e => e.MonthAgeId == id);
        }
    }
}
