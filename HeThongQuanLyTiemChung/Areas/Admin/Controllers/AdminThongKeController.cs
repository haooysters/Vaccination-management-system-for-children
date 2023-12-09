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
    public class AdminThongKeController : Controller
    {
        private readonly db_VaccineContext _context;

        public AdminThongKeController(db_VaccineContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TongDoanhThu = ThongKeDoanhThu();
            ViewBag.TongDonDangKy = ThongKeDonDangKy();
            ViewBag.TongHocVien = ThongKeHocVien();
            var db_ClothingContext = _context.OrderDetails
                .Include(o => o.Injection)
                .Include(o => o.Order)
                .Include(o => o.Vaccine);
            return View();
        }
        //Tổng doanh thu từ khi thành lập web
        public Decimal ThongKeDoanhThu()
        {
            decimal TongDoanhThu = _context.OrderDetails.Sum(n => n.Total).Value;

            return TongDoanhThu;
        }

        //Thống kê đơn đăng ký
        public Double ThongKeDonDangKy()
        {
            double ddk = _context.Orders.Count();

            return ddk;
        }

        //Thống kê tổng số học viên 
        public Double ThongKeHocVien()
        {
            double hv = _context.Customers.Count();

            return hv;
        }
    }
}
