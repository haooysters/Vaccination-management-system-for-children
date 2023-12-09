using HeThongQuanLyTiemChung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThemMuiTiemController : Controller
    {
        private readonly db_VaccineContext _context;

        public ThemMuiTiemController(db_VaccineContext context)
        {
            _context = context;
        }

        public IActionResult TaoMoi(int? id)
        {
            return PartialView("TaoMoi");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoMoi(Injection muitiem, int? id)
        {

            var lsTKH = _context.Diseases.Include(p => p.Injections).FirstOrDefault();
            //khoi tao don hang
            Injection injection = new Injection();
            injection.DiseaseId = id;
            injection.InjectionName = muitiem.InjectionName;
            injection.MonthAgeName = muitiem.MonthAgeName;

            _context.Add(injection);
            _context.SaveChanges();
            return PartialView("TaoMoi", injection);
        }
    }
}
