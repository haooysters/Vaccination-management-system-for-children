using AspNetCoreHero.ToastNotification.Abstractions;
using HeThongQuanLyTiemChung.Extension;
using HeThongQuanLyTiemChung.Helpper;
using HeThongQuanLyTiemChung.Models;
using HeThongQuanLyTiemChung.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly db_VaccineContext _context;

        //private readonly UserManager<Account> _userManager;

        public INotyfService _notyfService { get; }
        public AccountsController(db_VaccineContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (khachhang != null)
                    return Json(data: "Số điện thoại : " + Phone + "đã được sử dụng");

                return Json(data: false);

            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (khachhang != null)
                    return Json(data: "Email : " + Email + " đã được sử dụng");
                return Json(data: true);
            }
            catch
            {
                return Json(data: false);
            }
        }
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetInt32("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.Include(p => p.Gender).AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    var lsDonHang = _context.Orders
                     .AsNoTracking()
                     .Where(x => x.CustomerId == khachhang.CustomerId)
                     .OrderByDescending(x => x.CreateDate)
                     .ToList();
                    ViewBag.DonHang = lsDonHang;
                    return View(khachhang);
                }


            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public IActionResult DangkyTaiKhoan()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public async Task<IActionResult> DangkyTaiKhoan(RegisterViewModel taikhoan, [FromServices] ISendMailService dateTime)
        {
            if (ModelState.IsValid)
            {
                var check = _context.Accounts.FirstOrDefault(s => s.Email == taikhoan.Email);

                if (check == null)
                {


                    string salt = Utilities.GetRandomKey();
                    Customer khachhang = new Customer
                    {
                        FullName = taikhoan.FullName,
                        Phone = taikhoan.Phone.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                        //Active = true,
                        Salt = salt,
                        CreateDate = DateTime.Now,
                        EmailConfirmed = false

                    };


                    _context.Add(khachhang);
                    await _context.SaveChangesAsync();

                    //// trong email gửi đi để người dùng xác nhận
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(khachhang);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Action(
                       "ConfirmEmail", "Accounts", new { userId = khachhang.CustomerId },
                       protocol: Request.Scheme);


                    // Gửi email    
                    await dateTime.SendEmailAsync(khachhang.Email, "Xác nhận địa chỉ email",
                        $"Hãy xác nhận địa chỉ email bằng cách <a href='{callbackUrl}'>Bấm vào đây</a>.");


                    //    //Lưu Session MaKh
                    //    HttpContext.Session.SetString("CustomerId", khachhang.AccountId.ToString());
                    //    var taikhoanID = HttpContext.Session.GetString("CustomerId");

                    //    //Identity
                    //    var claims = new List<Claim>
                    //{
                    //    new Claim(ClaimTypes.Name,khachhang.FullName),
                    //    new Claim("CustomerId", khachhang.AccountId.ToString())
                    //};
                    //    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    //    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    //    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyfService.Error("Bạn cần xác thực tài khoản bằng Email");
                    return View("NotyfEmailConfirm");

                }
                else
                {
                    ViewBag.error = "Email đã được sử dụng";
                    return View(taikhoan);
                }
            }
            else
            {
                return View(taikhoan);
            }

        }
      
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public IActionResult Login(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetInt32("CustomerId");
            if (taikhoanID != null)
            {
                return RedirectToAction("Dashboard", "Accounts");
            }
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public async Task<IActionResult> Login(DangNhap customer, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    //if (!isEmail) return View(customer);

                    var khachhang = _context.Customers.Include(p => p.Gender).AsNoTracking().SingleOrDefault(x => x.Phone.Trim() == customer.Phone);

                    //if (khachhang == null) return RedirectToAction("DangkyTaiKhoan");
                    string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
                    if (khachhang.Password != pass)
                    {
                        _notyfService.Error("Thông tin đăng nhập chưa chính xác");
                        return View(customer);
                    }


                    //Luu Session MaKh
                    HttpContext.Session.SetInt32("CustomerId", khachhang.CustomerId);
                    var taikhoanID = HttpContext.Session.GetInt32("CustomerId");

                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, khachhang.Phone),
                        new Claim("CustomerId", khachhang.CustomerId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "dangnhap");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyfService.Success("Đăng nhập thành công");
                    return RedirectToAction("Dashboard", "Accounts");
                }
            }
            catch
            {
                return RedirectToAction("DangkyTaiKhoan", "Accounts");
            }
            return View(customer);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(int userId, string code)
        {
            var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == userId);


            //if (khachhang == null || code == null)
            //{
            //    return RedirectToPage("/Index");
            //}



            if (khachhang == null)
            {
                return NotFound($"Không tồn tại User - '{userId}'.");
            }

            //code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            //// Xác thực email
            //var result = await _userManager.ConfirmEmailAsync(khachhang, code);

            //if (result.Succeeded)
            //{

            // Đăng nhập luôn nếu xác thực email thành công
            //Luu Session MaKh
            HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
            var taikhoanID = HttpContext.Session.GetString("CustomerId");

            //Identity
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, khachhang.FullName),
                        new Claim("CustomerId", khachhang.CustomerId.ToString())
                    };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);

            khachhang.EmailConfirmed = true;
            _context.Update(khachhang);
            _context.SaveChanges();


            _notyfService.Success("Đăng nhập thành công");
            return RedirectToAction("Dashboard", "Accounts");
            //}
            //else
            //{
            //    _notyfService.Error("Lỗi xác nhận email");
            //}           
        }


        [HttpGet]
        [Route("dang-xuat-khach-hang.html", Name = "DangXuatKH")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("CustomerId");
            HttpContext.Session.Remove("CustomerId");
            _notyfService.Success("Đăng xuất thành công");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetInt32("CustomerId");

                if (taikhoanID == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }
                if (ModelState.IsValid)
                {
                    var taikhoan = _context.Customers.Find(Convert.ToInt32(taikhoanID));
                    if (taikhoan == null) return RedirectToAction("Login", "Accounts");
                    var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    {
                        string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                        taikhoan.Password = passnew;
                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notyfService.Success("Đổi mật khẩu thành công");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }
            }
            catch
            {
                _notyfService.Success("Thay đổi mật khẩu không thành công");
                return RedirectToAction("Dashboard", "Accounts");
            }
            _notyfService.Success("Thay đổi mật khẩu không thành công");
            return RedirectToAction("Dashboard", "Accounts");
        }
    }
}
