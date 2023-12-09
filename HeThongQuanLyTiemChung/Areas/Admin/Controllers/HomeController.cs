using HeThongQuanLyTiemChung.Models;
using HeThongQuanLyTiemChung.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff")]
    public class HomeController : Controller
    {
        private readonly db_VaccineContext _context;

        public HomeController(db_VaccineContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // GET: Movies
        public IActionResult TimKiem(string sKey, int txtCatID)
        {
            string tukhoa = sKey;

            List<Vaccine> lsCourses = new List<Vaccine>();

            if (!string.IsNullOrEmpty(sKey))
            {
                lsCourses = _context.Vaccines.AsNoTracking()
                                 .Include(p => p.Brand)
                                 //.Include(p => p.Cat)
                                 .Include(p => p.Age)
                                 .Where(x => x.VaccineName.Contains(sKey))                                 
                                 .OrderByDescending(x => x.VaccineName)
                                 .ToList();
            }
            else
            {
                lsCourses = _context.Vaccines
                .AsNoTracking()
                .Include(p => p.Brand)
                //.Include(p => p.Cat)
                .Include(p => p.Age)
                .OrderByDescending(x => x.VaccineId).ToList();
            }
            if (txtCatID != 0)
            {
                lsCourses = _context.Vaccines.AsNoTracking()
                                .Include(p => p.Brand)
                                //.Include(p => p.Cat)
                                .Include(p => p.Age)
                                //.Where(x => x.CatId == txtCatID)
                                .Where(x => x.VaccineName.Contains(sKey))
                                .OrderByDescending(x => x.VaccineName)
                                .ToList();
            }

            
            //var dsVaccine = from kh in Customer
            //                join hd in Order on kh.CustomerId equals hd.CustomerId
            //                join ct in OrderDetail on hd.OrderId equals ct.OrderId
            //                join vc in Vaccine on ct.VaccineId equals vc.VaccineId
            //var dsVaccine = (from kh in _context.Customers
            //                join hd in _context.Orders on kh.CustomerId equals hd.CustomerId
            //                join ct in _context.OrderDetails on hd.OrderId equals ct.OrderId
            //                join vc in _context.Vaccines on ct.VaccineId equals vc.VaccineId
            //                where kh.FullName == ""                         
            //                select new
            //                {
            //                    vc.VaccineId

            //                }).ToList();

            //var dsNotVaccine = dsVaccine.Where(ds => !dsVaccine.Any(db => db.VaccineId == ds.VaccineId)).ToList();

            //var dsChuaVaccine = _context.Vaccines.Where(ds => !dsVaccine.Any(db => db.VaccineId == ds.VaccineId))
            //    .Where(x => x.CatId == txtCatID).ToList();

            //ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewBag.tukhoa = tukhoa;
            ViewBag.Foo = "Bar";
            return View(lsCourses);
        }
    }
}
