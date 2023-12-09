using AspNetCoreHero.ToastNotification.Abstractions;
using HeThongQuanLyTiemChung.Extension;
using HeThongQuanLyTiemChung.Models;
using HeThongQuanLyTiemChung.ModelViews;
using HeThongQuanLyTiemChung.ModelViews.Payments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static HeThongQuanLyTiemChung.ModelViews.SendMailService;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CheckoutController : Controller
    {
        private readonly db_VaccineContext _context;


        private ISendMailService _mailService;
        public INotyfService _notyfService { get; }


        public IConfiguration Configuration { get; }

        private readonly IHttpContextAccessor Accessor;

        public CheckoutController(db_VaccineContext context, INotyfService notyfService, ISendMailService mailService, IHttpContextAccessor accessor, IConfiguration configuration)
        {
            _context = context;
            _notyfService = notyfService;
            _mailService = mailService;
            Accessor = accessor;
            Configuration = configuration;
        }
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            //Lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetInt32("KhachHang");
                   

            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.KhachHangId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
            }
            ViewBag.GioHang = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(MuaHangVM muaHang,[FromServices] ISendMailService dateTime)
        {
            //Lay ra gio hang de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetInt32("KhachHang");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.KhachHangId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;


                khachhang.Phone = muaHang.Phone;
                khachhang.Address = muaHang.Address;
                _context.Update(khachhang);
                _context.SaveChanges();
            }
            try
            {
                if (ModelState.IsValid)
                {

                    var code = new { Success = false, Code = -1, Url = "" };
                    //khoi tao don hang
                    Order donhang = new Order();
                    donhang.CustomerId = model.KhachHangId;
                    donhang.CreateDate = DateTime.Now;
                    donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.ThanhTien));

                    _context.Add(donhang);
                    _context.SaveChanges();
                    //tao danh sach don hang

                    foreach (var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = donhang.OrderId;
                        orderDetail.VaccineId = item.VaccineID;
                        orderDetail.InjectionId = item.InjectionID;
                        orderDetail.LotNumber = item.SoLo;
                        orderDetail.Dosage = item.LieuLuong;
                        orderDetail.Quantity = item.SoLuong;
                        orderDetail.Total = item.SoLuong * item.Price;
                        Random rd = new Random();
                        donhang.Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);


                        _context.Add(orderDetail);
                        //var hangHoa = _context.Products.SingleOrDefault(p => p.ProductId == orderDetail.ProductId);
                        //hangHoa.Amount -= item.SoLuong;
                        //_context.Update(hangHoa);
                        _context.SaveChanges();
                    }
                    _context.SaveChanges();

                    // Lấy dịch vụ sendmailservice


                    //var content = new MailContent();


                    //content.To = "haob1909909@student.ctu.edu.vn";
                    //content.Subject = "Kiểm tra thử";
                    //content.Body = "Họ và tên: " + model.FullName + "<br/>" + "Số điện thoại: " + model.Phone;

                    //dateTime.SendMail(content);

                    ////clear gio hang
                    HttpContext.Session.Remove("GioHang");

                    if (muaHang.TypePayment == 2)
                    {
                        var url = UrlPayment(2, donhang.Code);
                        code = new { Success = true, Code = 2, Url = url };
                        return Json(code);
                    }

                    //clear gio hang
                    HttpContext.Session.Remove("GioHang");
                    HttpContext.Session.Remove("KhachHang");
                    HttpContext.Session.Remove("GoiVaccine");
                    //Xuat thong bao
                    _notyfService.Success("Đăng ký thành công");
                    //cap nhat thong tin khach hang
                    return RedirectToAction("Index", "Home");


                }
            }
            catch
            {
                ViewBag.GioHang = cart;
                return View(model);
            }
            ViewBag.GioHang = cart;
            return View(model);
        }

        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = _context.Orders.FirstOrDefault(x => x.Code == orderCode);
            //Get Config Info
            string vnp_Returnurl = Configuration["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = Configuration["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = Configuration["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = Configuration["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalMoney * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }
            vnpay.AddRequestData("vnp_CreateDate", order.CreateDate.Value.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }

        [Route("vnpay_return", Name = "vnpay_return")]
        public ActionResult VnpayReturn()
        {
            if (Accessor.HttpContext.Request.Query.Count > 0)
            {
                // _notyfService.Success(Accessor.HttpContext.Request.Query.Count.ToString());
                string vnp_HashSecret = Configuration["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Accessor.HttpContext.Request.Query;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (var s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s.Value.ToString()) && s.Key.ToString().StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s.Key.ToString(), s.Value.ToString());
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId; long.TryParse(vnpay.GetResponseData("vnp_TransactionNo"), out vnpayTranId);
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Accessor.HttpContext.Request.Query["vnp_SecureHash"];
                String TerminalID = Accessor.HttpContext.Request.Query["vnp_TmnCode"];
                long vnp_Amount; long.TryParse(vnpay.GetResponseData("vnp_Amount"), out vnp_Amount);
                vnp_Amount = vnp_Amount / 100;
                String bankCode = Accessor.HttpContext.Request.Query["vnp_BankCode"];
                //_notyfService.Success(vnp_SecureHash);
                bool checkSignature = true;//vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                if (checkSignature)
                {

                }
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {

                    var itemOrder = _context.Orders.FirstOrDefault(x => x.Code == orderCode);
                    if (itemOrder != null)
                    {
                        // _notyfService.Success("ok1");
                        itemOrder.Paid = true; //đã thanh toán
                                               //context.Orders.Attach(itemOrder);
                                               //_context.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                    }
                    //Thanh toan thanh cong
                    ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ!";
                    //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    _notyfService.Success("Thanh toán thành công !");
                }
                else
                {
                    //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                    ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                    //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                }
                //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
            }
            return View();
        }
        //var a = UrlPayment(0, "DH3574");


        //[Route("dat-hang-thanh-cong.html", Name = "Success")]
        //public IActionResult Success()
        //{
        //    try
        //    {
        //        var taikhoanID = HttpContext.Session.GetString("CustomerId");
        //        if (string.IsNullOrEmpty(taikhoanID))
        //        {
        //            return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
        //        }
        //        var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountId == Convert.ToInt32(taikhoanID));
        //        var donhang = _context.Orders
        //            .Where(x => x.AccountId == Convert.ToInt32(taikhoanID))
        //            .OrderByDescending(x => x.CreateDate)
        //            .FirstOrDefault();
        //        MuaHangSuccessVM successVM = new MuaHangSuccessVM();
        //        successVM.FullName = khachhang.FullName;
        //        successVM.DonHangID = donhang.OrderId;
        //        successVM.Phone = khachhang.Phone;
        //        successVM.Address = khachhang.Address;


        //        return View(successVM);
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
