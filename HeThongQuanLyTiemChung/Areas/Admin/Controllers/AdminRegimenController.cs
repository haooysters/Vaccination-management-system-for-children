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
    public class AdminRegimenController : Controller
    {
        private readonly db_VaccineContext _context;

        public AdminRegimenController(db_VaccineContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminRegimen
        public async Task<IActionResult> Index()
        {
            var db_VaccineContext = _context.Regimens.Include(r => r.Injection).Include(r => r.MonthAge).Include(r => r.Vaccine);
            return View(await db_VaccineContext.ToListAsync());
        }

        // GET: Admin/AdminRegimen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regimen = await _context.Regimens
                .Include(r => r.Injection)
                .Include(r => r.MonthAge)
                .Include(r => r.Vaccine)
                //.Include(r => r.Vaccine.Cat)
                .FirstOrDefaultAsync(m => m.RegimenId == id);
            if (regimen == null)
            {
                return NotFound();
            }

            return View(regimen);
        }

        // GET: Admin/AdminRegimen/Create
        public IActionResult Create()
        {
            
            ViewData["Vacxin"] = new SelectList(_context.Vaccines, "VaccineId", "VaccineName");
            ViewData["ThangTuoi"] = new SelectList(_context.MonthAges, "MonthAgeId", "MonthAgeName");
            ViewData["MuiTiem"] = new SelectList(_context.Injections, "InjectionId", "InjectionName");
            
            return View();
        }

        // POST: Admin/AdminRegimen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegimenId,VaccineId,InjectionId,MonthAgeId")] Regimen regimen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(regimen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InjectionId"] = new SelectList(_context.Injections, "InjectionId", "InjectionId", regimen.InjectionId);
            ViewData["MonthAgeId"] = new SelectList(_context.MonthAges, "MonthAgeId", "MonthAgeId", regimen.MonthAgeId);
            ViewData["VaccineId"] = new SelectList(_context.Vaccines, "VaccineId", "VaccineId", regimen.VaccineId);
            return View(regimen);
        }

        // GET: Admin/AdminRegimen/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regimen = await _context.Regimens.FindAsync(id);
            if (regimen == null)
            {
                return NotFound();
            }
            ViewData["Vacxin"] = new SelectList(_context.Vaccines, "VaccineId", "VaccineName");
            ViewData["ThangTuoi"] = new SelectList(_context.MonthAges, "MonthAgeId", "MonthAgeName");
            ViewData["MuiTiem"] = new SelectList(_context.Injections, "InjectionId", "InjectionName");
            return View(regimen);
        }

        // POST: Admin/AdminRegimen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegimenId,VaccineId,InjectionId,MonthAgeId")] Regimen regimen)
        {
            if (id != regimen.RegimenId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(regimen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegimenExists(regimen.RegimenId))
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
            ViewData["InjectionId"] = new SelectList(_context.Injections, "InjectionId", "InjectionId", regimen.InjectionId);
            ViewData["MonthAgeId"] = new SelectList(_context.MonthAges, "MonthAgeId", "MonthAgeId", regimen.MonthAgeId);
            ViewData["VaccineId"] = new SelectList(_context.Vaccines, "VaccineId", "VaccineId", regimen.VaccineId);
            return View(regimen);
        }

        // GET: Admin/AdminRegimen/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regimen = await _context.Regimens
                .Include(r => r.Injection)
                .Include(r => r.MonthAge)
                .Include(r => r.Vaccine)
                .FirstOrDefaultAsync(m => m.RegimenId == id);
            if (regimen == null)
            {
                return NotFound();
            }

            return View(regimen);
        }

        // POST: Admin/AdminRegimen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var regimen = await _context.Regimens.FindAsync(id);
            _context.Regimens.Remove(regimen);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegimenExists(int id)
        {
            return _context.Regimens.Any(e => e.RegimenId == id);
        }
    }
}
