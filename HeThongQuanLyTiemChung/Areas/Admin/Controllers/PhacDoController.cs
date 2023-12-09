using HeThongQuanLyTiemChung.Models;
using Microsoft.AspNetCore.Http;
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
    public class PhacDoController : Controller
    {
        private readonly db_VaccineContext _context;

        public PhacDoController(db_VaccineContext context)
        {
            _context = context;
        }

        public IActionResult TaoMoi(int id)
        {

            //ViewBag.IDMuiTiem = id;

            HttpContext.Session.SetInt32("IDMuiTiem", id);

            HttpContext.Session.SetInt32("IDBenh", id);
            var benh = _context.Injections.Include(p => p.Disease).Where(p => p.InjectionId == id).FirstOrDefault();

            var lsVacine = _context.Vaccines.Where(p => p.DiseaseId == benh.DiseaseId).ToList();

            ViewBag.IDMuiTiem = benh;

            ViewData["Vaccine"] = new SelectList(lsVacine, "VaccineId", "VaccineName"); 
            return PartialView("TaoMoi");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoMoi(Injection muitiem)
        {

            //var id = HttpContext.Session.GetInt32("IDBenh");

            var lsTKH = _context.Diseases.Include(p => p.Injections).FirstOrDefault();
            //khoi tao don hang
            Injection injection = new Injection();
            //injection.DiseaseId = id;
            injection.InjectionName = muitiem.InjectionName;
            injection.MonthAgeName = muitiem.MonthAgeName;

            _context.Add(injection);
            _context.SaveChanges();

            //ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");
            return RedirectToAction("Index", "AdminDiseases");
        }
    }
}
