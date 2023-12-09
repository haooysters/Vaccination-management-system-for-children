using AspNetCoreHero.ToastNotification.Abstractions;
using HeThongQuanLyTiemChung.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HeThongQuanLyTiemChung.Extension;
using HeThongQuanLyTiemChung.Helpper;
using HeThongQuanLyTiemChung.ModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{    
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly db_VaccineContext _context;

        public INotyfService _notyfService { get; }

        public AccountController(db_VaccineContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [AllowAnonymous]
        //[Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("Staff");
            if (taikhoanID != null)
            {
                var khachhang = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountId == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login");
        }



        [HttpGet]
        [AllowAnonymous]
        //[Route("dang-ky.html", Name = "DangKy")]
        public IActionResult DangkyTaiKhoan()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[Route("dang-ky.html", Name = "DangKy")]
        public async Task<IActionResult> DangkyTaiKhoan(RegisterViewModel taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = _context.Accounts.FirstOrDefault(s => s.Email == taikhoan.Email);

                    if (check == null)
                    {


                        string salt = Utilities.GetRandomKey();
                        Account khachhang = new Account
                        {
                            FullName = taikhoan.FullName,
                            Phone = taikhoan.Phone.Trim().ToLower(),
                            Email = taikhoan.Email.Trim().ToLower(),
                            Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                            Active = true,
                            Salt = salt,
                            CreateDate = DateTime.Now,
                            RoleId = 2

                        };

                        try
                        {
                            _context.Add(khachhang);
                            await _context.SaveChangesAsync();
                           

                            //Identity
                            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name,khachhang.FullName),
                            new Claim("Staff", khachhang.AccountId.ToString())
                        };
                           
                            _notyfService.Success("Đăng ký thành công");
                            return RedirectToAction("Index", "AdminAccounts");
                        }
                        catch
                        {
                            return RedirectToAction("DangkyTaiKhoan", "Account");
                        }
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
            catch
            {
                return View(taikhoan);
            }
        }



        [HttpGet]
        [AllowAnonymous]
        //[Route("dang-nhap.html", Name = "DangNhap")]
        public IActionResult Login(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("Staff");
            if (taikhoanID != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[Route("dang-nhap.html", Name = "DangNhap")]
        public async Task<IActionResult> Login(LoginViewModel customer, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);

                    var khachhang = _context.Accounts.Include(p => p.Role).AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName);

                    if (khachhang == null)
                    {
                        _notyfService.Error("Thông tin đăng nhập chưa chính xác");
                        return View(customer);
                    }

                    string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
                    if (khachhang.Password != pass)
                    {
                        _notyfService.Error("Thông tin đăng nhập chưa chính xác");
                        return View(customer);
                    }
                    //kiem tra xem account co bi disable hay khong

                    if (khachhang.Active == false)
                    {
                        _notyfService.Error("Tài khoản của bạn đã bị khóa");
                        return View(customer);
                    }

                    //Luu Session MaKh
                    HttpContext.Session.SetString("Staff", khachhang.AccountId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("Staff");

                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, khachhang.FullName),
                        new Claim("Staff", khachhang.AccountId.ToString()),
                        new Claim("RoleId", khachhang.RoleId.ToString()),
                        new Claim(ClaimTypes.Role, khachhang.Role.RoleName)
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);

                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        _notyfService.Success("Đăng nhập thành công");
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            catch
            {
                return View(customer);
            }
            return View(customer);
        }

        [HttpGet]
        //[Route("admin-dang-xuat.html", Name = "DangXuat")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("Staff");
            HttpContext.Session.Remove("Staff");
            _notyfService.Success("Đăng xuất thành công");
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        //[Route("admin-dang-xuat.html", Name = "DangXuat")]
        public IActionResult AccessDenied()
        {
            HttpContext.Session.Remove("Staff");
            _notyfService.Error("Không có quyền truy cập!");
            return View();
        }


        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("Staff");
                if (taikhoanID == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var taikhoan = _context.Accounts.Find(Convert.ToInt32(taikhoanID));
                    if (taikhoan == null) return RedirectToAction("Login", "Account");
                    var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    {
                        string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                        taikhoan.Password = passnew;
                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notyfService.Success("Đổi mật khẩu thành công");
                        return RedirectToAction("Dashboard", "Account");
                    }
                }
            }
            catch
            {
                _notyfService.Success("Thay đổi mật khẩu không thành công");
                return RedirectToAction("Dashboard", "Account");
            }
            _notyfService.Success("Thay đổi mật khẩu không thành công");
            return RedirectToAction("Dashboard", "Account");
        }
    }
}
