using AspNetCoreHero.ToastNotification.Abstractions;
using HeThongQuanLyTiemChung.Helpper;
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

    public class TimKiemPhacDoController : Controller
    {
        private readonly db_VaccineContext _context;

        public INotyfService _notifyService { get; }

        public TimKiemPhacDoController(db_VaccineContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        public IActionResult TimKiem(string sKey, int txtCatID)
        {

            string tukhoa = sKey;

            // tim kiem khach hang the sdt
            var lsTKH = _context.Customers.Include(p => p.Gender).Where(n => n.Phone == sKey).FirstOrDefault();



            if (lsTKH != null)
            {
                //luu session makh
                HttpContext.Session.SetInt32("KhachHang", lsTKH.CustomerId);
                HttpContext.Session.Remove("GioHang");

                //int salt = Utilities.GetMonth(lsTKH.Birthday);

                //ds vac xin khach hang da tiem
                var dsCTVaccine = _context.OrderDetails.Where(n => n.Order.CustomerId == lsTKH.CustomerId);


                var phacdo = _context.Diseases.ToList();

                foreach (var item in phacdo)
                {
                    var muitiem = _context.Injections.Where(p => p.DiseaseId == item.DiseaseId);
                }

                    //ds vac xin khach hang da tiem cung loai
                    var lsVCCungLoai = _context.Regimens
                .AsNoTracking()
                .Include(p => p.MonthAge)
                .Include(p => p.Vaccine)
                .Include(p => p.Injection)
                //.Where(p => dsCTVaccine.Any(db => db.Regimen.Vaccine.AgeId == p.Vaccine.AgeId && db.Regimen.Vaccine.CatId == p.Vaccine.CatId && db.Regimen.InjectionId == p.InjectionId))
                .OrderByDescending(x => x.RegimenId);

                List<Regimen> lsVCGoi = new List<Regimen>();

                if (txtCatID != 0)
                {

                    lsVCGoi = _context.Regimens
                     .AsNoTracking()
                     .Include(p => p.Vaccine)
                     .Include(p => p.MonthAge)
                     .Include(p => p.Injection)
                     //.Where(x => x.Vaccine.CatId == txtCatID)
                     //.Where(p => p.MonthAge.MonthAgeName <= salt)
                     .Where(p => !lsVCCungLoai.Any(db => db.RegimenId == p.RegimenId))
                     .OrderByDescending(x => x.RegimenId).ToList();
                }
                else
                {
                    lsVCGoi = _context.Regimens
                    .AsNoTracking()
                    .Include(p => p.Vaccine)
                    .Include(p => p.MonthAge)
                    .Include(p => p.Injection)
                    //.Where(p => p.MonthAge.MonthAgeName <= salt)
                    .Where(p => !lsVCCungLoai.Any(db => db.RegimenId == p.RegimenId))
                    .OrderByDescending(x => x.RegimenId).ToList();
                }
                ViewBag.KhachHang = lsTKH;

                var lsDonHang = _context.Orders
                       .AsNoTracking()
                       .Where(x => x.CustomerId == lsTKH.CustomerId)
                       .OrderByDescending(x => x.CreateDate)
                       .ToList();
                ViewBag.DonHang = lsDonHang;

                //string thangtuoi = Utilities.GetMonthOfAge(salt);

                //ViewBag.ThangTuoi = thangtuoi;
                ViewBag.tukhoa = tukhoa;

                //ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
                return View(lsVCGoi);

            }
            else
            {
                _notifyService.Error("Không tìm thấy thông tin khách hàng nào!");
                return RedirectToAction("Index", "Home");
            }


        }
    }
}
