using AspNetCoreHero.ToastNotification.Abstractions;
using HeThongQuanLyTiemChung.Helpper;
using HeThongQuanLyTiemChung.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.Controllers
{
    public class LichSuTiemChungController : Controller
    {
        private readonly db_VaccineContext _context;

        public INotyfService _notifyService { get; }

        public LichSuTiemChungController(db_VaccineContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        public IActionResult Index(string sKey)
        {
            // tim kiem khach hang the sdt
            var lsTKH = _context.Customers.Include(p => p.Gender).Where(n => n.Phone == sKey).FirstOrDefault();

            if (lsTKH != null)
            {
                //Luu Session MaKh
                HttpContext.Session.SetInt32("Customer", lsTKH.CustomerId);

                var lsDonHang = _context.Orders                       
                        .AsNoTracking()
                        .Where(x => x.CustomerId == lsTKH.CustomerId)
                        .OrderByDescending(x => x.CreateDate)
                        .ToList();
                ViewBag.DonHang = lsDonHang;

                //int salt = Utilities.GetMonth(lsTKH.Birthday);

                //ds vac xin khach hang da tiem
                var dsCTVaccine = _context.OrderDetails.Where(n => n.Order.CustomerId == lsTKH.CustomerId);




                ////ds vac xin khach hang da tiem cung loai
                //var lsVCCungLoai = _context.Regimens
                //.AsNoTracking()
                //.Include(p => p.MonthAge)
                //.Include(p => p.Vaccine)
                //.Include(p => p.Injection)
                ////.Where(p => dsCTVaccine.Any(db => db.Regimen.Vaccine.AgeId == p.Vaccine.AgeId && db.Regimen.Vaccine.CatId == p.Vaccine.CatId && db.Regimen.InjectionId == p.InjectionId))
                //.OrderByDescending(x => x.RegimenId);

                //List<Regimen> lsVCGoi = new List<Regimen>();

               

              
                    //lsVCGoi = _context.Regimens
                    //.AsNoTracking()
                    //.Include(p => p.Vaccine)
                    //.Include(p => p.MonthAge)
                    //.Include(p => p.Injection)
                    ////.Where(p => p.MonthAge.MonthAgeName <= salt)
                    //.Where(p => !lsVCCungLoai.Any(db => db.RegimenId == p.RegimenId))
                    //.OrderByDescending(x => x.RegimenId).ToList();



                //ViewBag.PhacDo = lsVCGoi;


                return View(lsTKH);                
            }


            _notifyService.Error("Không tìm thấy thông tin khách hàng nào!");
            return RedirectToAction("Index", "Home");
        }


        //[HttpGet]
        //[Route("dang-xuat.html", Name = "DangXuat")]
        //public IActionResult Logout()
        //{
        //    HttpContext.SignOutAsync();
        //    HttpContext.Session.Remove("Customer");
        //    _notifyService.Success("Thoát thành công");
        //    return RedirectToAction("Index", "Home");
        //}
    }
}
