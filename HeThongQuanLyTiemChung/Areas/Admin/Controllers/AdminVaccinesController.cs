using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyTiemChung.Models;
using HeThongQuanLyTiemChung.Helpper;
using System.IO;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminVaccinesController : Controller
    {
        private readonly db_VaccineContext _context;

        public INotyfService _notifyService { get; }

        public AdminVaccinesController(db_VaccineContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }




        // GET: Admin/AdminVaccines
        public IActionResult Index(string keyword, int CatID = 0)
        {
            List<Vaccine> lsCourses = new List<Vaccine>();

            lsCourses = _context.Vaccines.AsNoTracking()
                                 .Include(p => p.Brand)
                                 .Include(p => p.Disease)
                                 .Include(p => p.Age)                                
                                 
                                 .OrderByDescending(x => x.VaccineId)
                                 .ToList();

            //if (!string.IsNullOrEmpty(keyword))
            //{
            //    lsCourses = _context.Vaccines.AsNoTracking()
            //                     .Include(p => p.Brand)
            //                     .Include(p => p.Cat)
            //                     .Include(p => p.Age)
            //                     .Include(p => p.Injection)
            //                     .Where(x => x.VaccineName.Contains(keyword))
            //                     .Where(x => x.CatId == CatID)
            //                     .OrderByDescending(x => x.VaccineName)
            //                     .ToList();
            //}
            //else
            //{
            //    lsCourses = _context.Vaccines
            //    .AsNoTracking()
            //    .Include(p => p.Brand)
            //    .Include(p => p.Cat)
            //    .Include(p => p.Age)
            //    .Include(p => p.Injection)
            //    .OrderByDescending(x => x.VaccineId).ToList();
            //}

            //if (CatID != 0)
            //{

            //    lsCourses = _context.Vaccines
            //    .AsNoTracking()
            //    .Where(x => x.CatId == CatID)
            //    .Include(p => p.Brand)
            //    .Include(p => p.Cat)
            //    .Include(p => p.Age)
            //    .Include(p => p.Injection)
            //    .OrderByDescending(x => x.VaccineId).ToList();
            //}
            //else
            //{
            //    lsCourses = _context.Vaccines
            //    .AsNoTracking()
            //    .Include(p => p.Brand)
            //    .Include(p => p.Cat)
            //    .Include(p => p.Age)
            //    .Include(p => p.Injection)
            //    .OrderByDescending(x => x.VaccineId).ToList();
            //}

            var db_VaccineContext = _context.Vaccines.Include(v => v.Age).Include(v => v.Brand);

            ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseID", "DiseaseName");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["NhomTuoi"] = new SelectList(_context.Ages, "AgeId", "AgeName");
            ViewData["MuiTiem"] = new SelectList(_context.Injections, "InjectionId", "InjectionName");
            ViewData["GoiVaccine"] = new SelectList(_context.Packages, "PackageId", "PackageName");

            return View(lsCourses);
        }

        public IActionResult Filtter(int CatID = 0)
        {
            var url = $"/Admin/AdminVaccines?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Admin/AdminVaccines";
            }
            return Json(new { status = "success", redirectUrl = url });
        }


        // GET: Admin/AdminVaccines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                
                return NotFound();
            }

            var vaccine = await _context.Vaccines
                .Include(v => v.Age)
                .Include(v => v.Brand)
                .Include(p => p.Disease)
                //.Include(p => p.Injection)
                //.Include(p => p.Package)
                .FirstOrDefaultAsync(m => m.VaccineId == id);
            if (vaccine == null)
            {
                return NotFound();
            }

            ViewData["MuiTiem"] = new SelectList(_context.Injections, "InjectionId", "InjectionName");

            return View(vaccine);
        }

        // GET: Admin/AdminVaccines/Create
        public IActionResult Create()
        {
            //ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["NhomTuoi"] = new SelectList(_context.Ages, "AgeId", "AgeName");
            ViewData["MuiTiem"] = new SelectList(_context.Injections, "InjectionId", "InjectionName");
            ViewData["GoiVaccine"] = new SelectList(_context.Packages, "PackageId", "PackageName");
            ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");

            return View();
        }

        // POST: Admin/AdminVaccines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VaccineId,VaccineName,ShortDesc,Description,AgeId,BrandId,DiseaseId,InjectionId,PackageId,Active,Price,Thumb,CreateDate,ModifiedDate")] Vaccine vaccine, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                vaccine.VaccineName = Utilities.ToTitleCase(vaccine.VaccineName);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(vaccine.VaccineName) + extension;
                    vaccine.Thumb = await Utilities.UploadFile(fThumb, @"vaccines", image.ToLower());
                }
                if (string.IsNullOrEmpty(vaccine.Thumb)) vaccine.Thumb = "default.jpg";

                vaccine.CreateDate = DateTime.Now;


                _context.Add(vaccine);
                await _context.SaveChangesAsync();
                _notifyService.Success("Thêm mới thành công");
                return RedirectToAction(nameof(Index));
            }
            //ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["NhomTuoi"] = new SelectList(_context.Ages, "AgeId", "AgeName");
            ViewData["GoiVaccine"] = new SelectList(_context.Packages, "PackageId", "PackageName");
            ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");

            return View(vaccine);
        }

        // GET: Admin/AdminVaccines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vaccine = await _context.Vaccines.FindAsync(id);
            if (vaccine == null)
            {
                return NotFound();
            }
            //ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["NhomTuoi"] = new SelectList(_context.Ages, "AgeId", "AgeName");
            ViewData["MuiTiem"] = new SelectList(_context.Injections, "InjectionId", "InjectionName");
            ViewData["GoiVaccine"] = new SelectList(_context.Packages, "PackageId", "PackageName");
            ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");


            return View(vaccine);
        }

        // POST: Admin/AdminVaccines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VaccineId,VaccineName,ShortDesc,Description,AgeId,BrandId,DiseaseId,InjectionId,PackageId,Active,Price,Thumb,CreateDate,ModifiedDate")] Vaccine vaccine, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != vaccine.VaccineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    vaccine.VaccineName = Utilities.ToTitleCase(vaccine.VaccineName);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(vaccine.VaccineName) + extension;
                        vaccine.Thumb = await Utilities.UploadFile(fThumb, @"vaccines", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(vaccine.Thumb)) vaccine.Thumb = "default.jpg";

                    vaccine.ModifiedDate = DateTime.Now;


                    _context.Update(vaccine);
                    _notifyService.Success("Cập nhật thành công");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VaccineExists(vaccine.VaccineId))
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
            ViewData["AgeId"] = new SelectList(_context.Ages, "AgeId", "AgeId", vaccine.AgeId);
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", vaccine.BrandId);
            //ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatId", vaccine.CatId);
            ViewData["GoiVaccine"] = new SelectList(_context.Packages, "PackageId", "PackageName");
            ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");

            return View(vaccine);
        }

        // GET: Admin/AdminVaccines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vaccine = await _context.Vaccines
                .Include(v => v.Age)
                .Include(v => v.Brand)
                .Include(v => v.Disease)
                .FirstOrDefaultAsync(m => m.VaccineId == id);
            if (vaccine == null)
            {
                return NotFound();
            }

            return View(vaccine);
        }

        // POST: Admin/AdminVaccines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vaccine = await _context.Vaccines.FindAsync(id);
            _context.Vaccines.Remove(vaccine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VaccineExists(int id)
        {
            return _context.Vaccines.Any(e => e.VaccineId == id);
        }
    }
}
