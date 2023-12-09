using AspNetCoreHero.ToastNotification.Abstractions;
using HeThongQuanLyTiemChung.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.Controllers
{
    public class TimKiemVaccineController : Controller
    {
        private readonly db_VaccineContext _context;

        public INotyfService _notifyService { get; }

        public TimKiemVaccineController(db_VaccineContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        [Route("search.html", Name = "Search")]
        public IActionResult KQSearch(string sKey)
        {

            // tim kiem theo ten khoa hoc
            var lsTKH = _context.Vaccines.Where(n => n.VaccineName.Contains(sKey));

            if (lsTKH.Count() == 0)
            {
                _notifyService.Error("Không tìm thấy vắc xin nào");
                return RedirectToAction("Index", "Home");
            }
            return View(lsTKH.OrderBy(n => n.VaccineName));
        }
    }
}
