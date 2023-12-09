using AspNetCoreHero.ToastNotification.Abstractions;
using HeThongQuanLyTiemChung.Extension;
using HeThongQuanLyTiemChung.Models;
using HeThongQuanLyTiemChung.ModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CartController : Controller
    {
        private readonly db_VaccineContext _context;
        public INotyfService _notifyService { get; }

        private readonly string _clientId;
        private readonly string _secretKey;

        public double TyGiaUSD = 23300;//store in Database

        public CartController(db_VaccineContext context, INotyfService notifyService, IConfiguration config)
        {
            _context = context;
            _notifyService = notifyService;
            _clientId = config["PaypalSettings:ClientId"];
            _secretKey = config["PaypalSettings:SecretKey"];
        }

        //Khởi tạo giỏ hàng
        public List<CartItem> Carts
        {
            get
            {

                var data = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (data == null)
                {
                    data = new List<CartItem>();
                }
                return data;
            }
        }

        public IActionResult Index()
        {
            ViewBag.Error = "";
            return View(Carts);
        }

        public IActionResult AddToCart(int idvaccine, string lieuluong, string solo, string type = "Normal")
        {
            var id = HttpContext.Session.GetInt32("IDMuiTiem");

            var myCart = Carts;
            var item = myCart.SingleOrDefault(p => p.InjectionID == id);
            var vaccine = _context.Vaccines.SingleOrDefault(p => p.VaccineId == idvaccine);



            ////Kiểm tra khóa học active ?
            //var check = _context.Vaccines
            //    .Where(s => s.VaccineId == id && s.Active == true).FirstOrDefault();
            ////Kiểm tra số lượng nhập vào
            //var checkSl = _context.Vaccines.AsNoTracking().SingleOrDefault(s => s.VaccineId == id);
            //if (check != null)
            //{
            //    if (checkSl != null)
            //    {
            var hangHoa = _context.Injections                      
                      .Include(p => p.Disease)                     
                      .SingleOrDefault(p => p.InjectionId == id);
            

          

            string muitiem, hinhanh;
            int giaban;

            //if (hangHoa.Thumb != null)
            //{
            //    hinhanh = hangHoa.Thumb;
            //}
            //else
            //{
            //    hinhanh = "Không có";
            //}
            //if (check == null)
            //{
            //    giaban = hangHoa.Price.Value;
            //}
            //else
            //{
            //    giaban = 0;
            //}


            //if (hangHoa.InjectionId != null)
            //{
            //    muitiem = hangHoa.Injection.InjectionName;                   
            //}
            //else
            //{
            //    muitiem = "Không có";
            //}


            if (item == null)//chưa có
            {

                item = new CartItem
                {
                    VaccineID = idvaccine,
                    VaccineName = vaccine.VaccineName,
                    Price = vaccine.Price,
                    SoLuong = 1,
                    Thumb = vaccine.Thumb,
                    InjectionID = hangHoa.InjectionId,
                    InjectionName = hangHoa.InjectionName,
                    DiseaseName = hangHoa.Disease.DiseaseName,
                    LieuLuong = lieuluong,
                    SoLo = solo

                };
                myCart.Add(item);

            }
            else
            {

                _notifyService.Error("Đã có trong giỏ hàng!");
                return RedirectToAction("TimKiem", "Search");

                //item.SoLuong += SoLuong;
            }
            HttpContext.Session.Set("GioHang", myCart);
            _notifyService.Success("Thêm giỏ hàng thành công!");


            if (type == "ajax")
            {
                return Json(new
                {
                    SoLuong = Carts.Sum(c => c.SoLuong)
                });
            }
            //    }
            //    else
            //    {
            //        _notifyService.Error("Sản phẩm hết hàng!");
            //    }
            //}
            //else
            //{
            //    _notifyService.Error("Sản phẩm hết hàng!");
            //}
            return RedirectToAction("Index", "Cart");
        }

        //Xóa giỏ hàng
        [HttpPost]
        //[Route("api/cart/remove")]
        public ActionResult Remove(int productID, int sizeID)
        {

            try
            {
                List<CartItem> gioHang = Carts;
                CartItem item = gioHang.SingleOrDefault(p => p.VaccineID == productID && p.InjectionID == sizeID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                //luu lai session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);

                

                _notifyService.Error("Xóa giỏ hàng thành công!");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        //Cập nhật giỏ hàng

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int sizeID, int? amount)
        {
            var checkSl = _context.Vaccines.AsNoTracking().SingleOrDefault(s => s.VaccineId == productID);

            //Lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (checkSl != null)
                {
                    if (cart != null)
                    {
                        CartItem item = cart.SingleOrDefault(p => p.VaccineID == productID && p.InjectionID == sizeID);
                        if (item != null && amount.HasValue) // da co -> cap nhat so luong
                        {
                            item.SoLuong = amount.Value;
                        }
                        //Luu lai session
                        HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                    }
                }
                else
                {
                    _notifyService.Error("Sản phẩm hết hàng!");
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}
