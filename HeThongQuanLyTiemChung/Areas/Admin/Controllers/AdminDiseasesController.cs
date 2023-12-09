using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyTiemChung.Models;
using HeThongQuanLyTiemChung.ModelViews;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminDiseasesController : Controller
    {
        private readonly db_VaccineContext _context;

        public AdminDiseasesController(db_VaccineContext context)
        {
            _context = context;
        }



        // GET: Admin/AdminDiseases
        public IActionResult Index()
        {

            List<MuiBenh> muiBenhs1 = new List<MuiBenh>();

            var lsBenh = _context.Diseases.Include(p => p.Injections).ToList();
            var lsMui = _context.Injections.ToList();



            foreach (var item in lsBenh)
            {
                foreach (var mui in lsMui)
                {
                    if (item.DiseaseId == mui.DiseaseId)
                    {
                        MuiBenh muiBenhs = new MuiBenh
                        {
                            InjectionId = mui.InjectionId,
                            DiseaseId = item.DiseaseId,
                            DiseaseName = item.DiseaseName,
                            InjectionName = mui.InjectionName,
                            MonthAgeName = mui.MonthAgeName

                        };
                        muiBenhs1.Add(muiBenhs);
                    }
                   
                }
                
            }


            ViewBag.Benh = muiBenhs1;


            return View(lsBenh);
        }

        // GET: Admin/AdminDiseases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disease = await _context.Diseases
                .FirstOrDefaultAsync(m => m.DiseaseId == id);
            if (disease == null)
            {
                return NotFound();
            }

            var benh = _context.Injections.Include(p => p.Disease).Where(p => p.DiseaseId == id).ToList();


            ViewBag.IDMuiTiem = benh;

            return View(disease);
        }

        // GET: Admin/AdminDiseases/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminDiseases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiseaseId,DiseaseName,Description")] Disease disease)
        {
            if (ModelState.IsValid)
            {
                _context.Add(disease);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(disease);
        }

        public IActionResult TaoMoi()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoMoi(Disease benh)
        {

            var lsTKH = _context.Diseases.Include(p => p.Injections).FirstOrDefault();
            //khoi tao don hang
            Disease disease = new Disease();
            disease.DiseaseName = benh.DiseaseName;


            _context.Add(disease);
            _context.SaveChanges();
            return View(disease);
        }


        // GET: Admin/AdminDiseases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disease = await _context.Diseases.FindAsync(id);
            if (disease == null)
            {
                return NotFound();
            }

            var benh = _context.Injections.Include(p => p.Disease).Where(p => p.DiseaseId == id).ToList();


            ViewBag.IDMuiTiem = benh;

            return View(disease);
        }

        // POST: Admin/AdminDiseases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiseaseId,DiseaseName,Description")] Disease disease)
        {
            if (id != disease.DiseaseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disease);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiseaseExists(disease.DiseaseId))
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
            return View(disease);
        }

        // GET: Admin/AdminDiseases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disease = await _context.Diseases
                .FirstOrDefaultAsync(m => m.DiseaseId == id);
            if (disease == null)
            {
                return NotFound();
            }

            return View(disease);
        }

        // POST: Admin/AdminDiseases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var disease = await _context.Diseases.FindAsync(id);
            _context.Diseases.Remove(disease);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiseaseExists(int id)
        {
            return _context.Diseases.Any(e => e.DiseaseId == id);
        }
    }
}
