using AspNetCoreHero.ToastNotification.Abstractions;
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
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NgayHenController : Controller
    {
        private readonly db_VaccineContext _context;

        private readonly IHttpContextAccessor Accessor;
        public IConfiguration Configuration { get; }

        public INotyfService _notifyService { get; }
        public NgayHenController(db_VaccineContext context, INotyfService notifyService, IHttpContextAccessor accessor, IConfiguration configuration)
        {
            _context = context;
            _notifyService = notifyService;
            Accessor = accessor;
            Configuration = configuration;
        }

        public IActionResult TaoMoi(int id)
        {

            HttpContext.Session.SetInt32("IDMuiTiem", id);

            return PartialView("TaoMoi");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoMoi([Bind("AppointDateId,InjectionId,CustomerId,AppointmentDate")] AppointDate appointDate)
        {

            var id = HttpContext.Session.GetInt32("IDMuiTiem");

            var taikhoanID = HttpContext.Session.GetInt32("KhachHang");
            var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));

            var checkngayhen = _context.AppointDates.Where(x => x.InjectionId == id && x.CustomerId == khachhang.CustomerId).FirstOrDefault();
            //var lsTKH = _context.Diseases.Include(p => p.Injections).FirstOrDefault();
            //khoi tao don hang
            if (checkngayhen == null)
            {
                AppointDate ngayhen = new AppointDate();
                ngayhen.CustomerId = khachhang.CustomerId;
                ngayhen.InjectionId = id;
                ngayhen.AppointmentDate = appointDate.AppointmentDate;
                _context.Add(ngayhen);
                _context.SaveChanges();
            }
            else
            {

                checkngayhen.AppointmentDate = appointDate.AppointmentDate;
                _context.Update(checkngayhen);
                _context.SaveChanges();
            }



            //ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");
            return RedirectToAction("TimKiem", "Search");
        }


        public IActionResult ChangeStatus(int id)
        {

            HttpContext.Session.SetInt32("Order", id);

            return PartialView("ChangeStatus");
        }
        [HttpPost]
        
        public IActionResult ChangeStatus(int id, ThanhToan order)
        {
            

            var idoder = HttpContext.Session.GetInt32("Order");

            var code = new { Success = false, Code = -1, Url = "" };

            var donhang = _context.Orders.AsNoTracking().FirstOrDefault(x => x.OrderId == idoder);
            if (donhang != null)
            {
                donhang.Paid = true;
                Random rd = new Random();
                donhang.Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
            }
            _context.Update(donhang);
            _context.SaveChanges();

            if (order.TypePayment == 2)
            {
                var url = UrlPayment(2, donhang.Code);
                code = new { Success = true, Code = 2, Url = url };
                return Json(code);
            }


            _notifyService.Success("Thanh toán thành công");

            return RedirectToAction("Index", "AdminOrder");
        }

        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = _context.Orders.Where(x => x.Code == orderCode).FirstOrDefault();
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

        //[Route("vnpay_return", Name = "vnpay_return")]
        //public ActionResult VnpayReturn()
        //{
        //    if (Accessor.HttpContext.Request.Query.Count > 0)
        //    {
        //        // _notyfService.Success(Accessor.HttpContext.Request.Query.Count.ToString());
        //        string vnp_HashSecret = Configuration["vnp_HashSecret"]; //Chuoi bi mat
        //        var vnpayData = Accessor.HttpContext.Request.Query;
        //        VnPayLibrary vnpay = new VnPayLibrary();

        //        foreach (var s in vnpayData)
        //        {
        //            //get all querystring data
        //            if (!string.IsNullOrEmpty(s.Value.ToString()) && s.Key.ToString().StartsWith("vnp_"))
        //            {
        //                vnpay.AddResponseData(s.Key.ToString(), s.Value.ToString());
        //            }
        //        }
        //        string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
        //        long vnpayTranId; long.TryParse(vnpay.GetResponseData("vnp_TransactionNo"), out vnpayTranId);
        //        string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        //        string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
        //        String vnp_SecureHash = Accessor.HttpContext.Request.Query["vnp_SecureHash"];
        //        String TerminalID = Accessor.HttpContext.Request.Query["vnp_TmnCode"];
        //        long vnp_Amount; long.TryParse(vnpay.GetResponseData("vnp_Amount"), out vnp_Amount);
        //        vnp_Amount = vnp_Amount / 100;
        //        String bankCode = Accessor.HttpContext.Request.Query["vnp_BankCode"];
        //        //_notyfService.Success(vnp_SecureHash);
        //        bool checkSignature = true;//vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

        //        if (checkSignature)
        //        {

        //        }
        //        if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
        //        {

        //            var itemOrder = _context.Orders.FirstOrDefault(x => x.Code == orderCode);
        //            if (itemOrder != null)
        //            {
        //                // _notyfService.Success("ok1");
        //                itemOrder.Paid = true; //đã thanh toán
        //                                       //context.Orders.Attach(itemOrder);
        //                                       //_context.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
        //                _context.SaveChanges();
        //            }
        //            //Thanh toan thanh cong
        //            ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
        //            //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
        //            _notifyService.Success("Thanh toán thành công !");
        //        }
        //        else
        //        {
        //            //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
        //            ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
        //            //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
        //        }
        //        //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
        //        //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
        //        //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
        //        ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
        //        //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
        //    }
        //    return View();
        //}
        ////var a = UrlPayment(0, "DH3574");
    }
}
