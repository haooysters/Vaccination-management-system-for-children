using HeThongQuanLyTiemChung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.Controllers
{
    public class VaccineController : Controller
    {
        private readonly db_VaccineContext _context;

        public VaccineController(db_VaccineContext context)
        {
            _context = context;
        }

        [Route("vaccine.html", Name = "Vaccine")]
        public IActionResult Index()
        {
            int i;
            //List<Vaccine> lsVaccine = new List<Vaccine>();         

            var lsVaccine = _context.Vaccines
            .AsNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Disease)
            .Include(p => p.Age)
            //.Include(p => p.Injection)
            .OrderBy(x => x.VaccineId);


            //var distinctPeople = _context.Vaccines.AsNoTracking()
            //.Include(p => p.Brand)
            //.Include(p => p.Cat)
            //.Include(p => p.Age)
            //.Include(p => p.Injection)
            //  .GroupBy(p => new { p.VaccineId, p.Price })
            //  .Select(g => g.First())
            //  .ToList();


            var db_VaccineContext = _context.Vaccines.Include(v => v.Age).Include(v => v.Brand);

            //ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");


            return View(lsVaccine);
        }

        [Route("/{VaccineName}-{id}.html", Name = "VaccineDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _context.Vaccines
                .Include(p => p.Brand)
                .Include(p => p.Disease)
                .Include(p => p.Age)

                .FirstOrDefault(x => x.VaccineId == id);

                if (product == null)
                {
                    return RedirectToAction("Index");
                }

                var lsProduct = _context.Vaccines
                    .AsNoTracking()
                    //.Where(x => x.CatId == product.CatId && x.VaccineId != id)
                    .Take(2)
                    .OrderByDescending(x => x.CreateDate)
                    .ToList();
                ViewBag.SanPham = lsProduct;

                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
