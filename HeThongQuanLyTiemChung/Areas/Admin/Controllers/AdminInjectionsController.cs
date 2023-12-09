using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyTiemChung.Models;
using Microsoft.AspNetCore.Http;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminInjectionsController : Controller
    {
        private readonly db_VaccineContext _context;

        public AdminInjectionsController(db_VaccineContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminInjections
        public async Task<IActionResult> Index()
        {
            return View(await _context.Injections.Include(p => p.Disease).ToListAsync());
        }

        // GET: Admin/AdminInjections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var injection = await _context.Injections
                .FirstOrDefaultAsync(m => m.InjectionId == id);
            if (injection == null)
            {
                return NotFound();
            }

            return View(injection);
        }

        // GET: Admin/AdminInjections/Create
        public IActionResult Create()
        {
            ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");
            return View();
        }

        // POST: Admin/AdminInjections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InjectionId,DiseaseId,InjectionName,MonthAgeName")] Injection injection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(injection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(injection);
        }

        public IActionResult TaoMoi(int id)
        {

            ViewBag.IDBenh = id;
            var lsTKH = _context.Diseases.Include(p => p.Injections).Where(p => p.DiseaseId == id).FirstOrDefault();

            ViewBag.Benh = lsTKH;

            HttpContext.Session.SetInt32("IDBenh", id);

            ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");
            return PartialView("TaoMoi");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoMoi([Bind("InjectionId,DiseaseId,InjectionName,MonthAgeName")] Injection muitiem)
        {

            var id = HttpContext.Session.GetInt32("IDBenh");

            var lsTKH = _context.Diseases.Include(p => p.Injections).FirstOrDefault();
            //khoi tao don hang
            Injection injection = new Injection();
            injection.DiseaseId = id;
            injection.InjectionName = muitiem.InjectionName;
            injection.MonthAgeName = muitiem.MonthAgeName;


            

            _context.Add(injection);
            _context.SaveChanges();

            ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");
            return RedirectToAction("Index", "AdminDiseases");
        }


        // GET: Admin/AdminInjections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var injection = await _context.Injections.FindAsync(id);
            if (injection == null)
            {
                return NotFound();
            }

            ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");

            return View(injection);
        }

        // POST: Admin/AdminInjections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InjectionId,DiseaseId,InjectionName,MonthAgeName")] Injection injection)
        {
            if (id != injection.InjectionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(injection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InjectionExists(injection.InjectionId))
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
            return View(injection);
        }

        // GET: Admin/AdminInjections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var injection = await _context.Injections.Include(p => p.Disease)
                .FirstOrDefaultAsync(m => m.InjectionId == id);
            if (injection == null)
            {
                return NotFound();
            }

            return View(injection);
        }

        // POST: Admin/AdminInjections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var injection = await _context.Injections.FindAsync(id);
            _context.Injections.Remove(injection);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InjectionExists(int id)
        {
            return _context.Injections.Any(e => e.InjectionId == id);
        }
    }
}
