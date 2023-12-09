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
    public class AdminAgesController : Controller
    {
        private readonly db_VaccineContext _context;

        public AdminAgesController(db_VaccineContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminAges
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ages.ToListAsync());
        }

        // GET: Admin/AdminAges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var age = await _context.Ages
                .FirstOrDefaultAsync(m => m.AgeId == id);
            if (age == null)
            {
                return NotFound();
            }

            return View(age);
        }

        // GET: Admin/AdminAges/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminAges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AgeId,AgeName")] Age age)
        {
            if (ModelState.IsValid)
            {
                _context.Add(age);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(age);
        }

        // GET: Admin/AdminAges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var age = await _context.Ages.FindAsync(id);
            if (age == null)
            {
                return NotFound();
            }
            return View(age);
        }

        // POST: Admin/AdminAges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AgeId,AgeName")] Age age)
        {
            if (id != age.AgeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(age);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgeExists(age.AgeId))
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
            return View(age);
        }

        // GET: Admin/AdminAges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var age = await _context.Ages
                .FirstOrDefaultAsync(m => m.AgeId == id);
            if (age == null)
            {
                return NotFound();
            }

            return View(age);
        }

        // POST: Admin/AdminAges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var age = await _context.Ages.FindAsync(id);
            _context.Ages.Remove(age);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgeExists(int id)
        {
            return _context.Ages.Any(e => e.AgeId == id);
        }
    }
}
