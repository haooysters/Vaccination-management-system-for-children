using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyTiemChung.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using HeThongQuanLyTiemChung.Helpper;
using HeThongQuanLyTiemChung.Extension;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCustomersController : Controller
    {
        private readonly db_VaccineContext _context;

        public INotyfService _notifyService { get; }

        public AdminCustomersController(db_VaccineContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/AdminCustomers
        public async Task<IActionResult> Index()
        {
            var db_VaccineContext = _context.Customers.Include(c => c.Gender);

            //ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["NhomTuoi"] = new SelectList(_context.Ages, "AgeId", "AgeName");
            ViewData["MuiTiem"] = new SelectList(_context.Injections, "InjectionId", "InjectionName");
            ViewData["GoiVaccine"] = new SelectList(_context.Packages, "PackageId", "PackageName");

            return View(await db_VaccineContext.ToListAsync());
        }

        // GET: Admin/AdminCustomers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Gender)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/AdminCustomers/Create
        public IActionResult Create()
        {

            ViewData["GioiTinh"] = new SelectList(_context.Genders, "GenderId", "GenderName");
            return View();
        }

        // POST: Admin/AdminCustomers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,GenderId,FullName,Birthday,Salt,Password,Address,Email,Phone,CreateDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {

                var check = _context.Customers.FirstOrDefault(s => s.Phone == customer.Phone);
                
                if (check == null)
                {



                    string salt = Utilities.GetRandomKey();
                    Customer khachhang = new Customer
                    {
                        FullName = customer.FullName,
                        Phone = customer.Phone.Trim().ToLower(),
                        Email = customer.Email.Trim().ToLower(),
                        Password = (customer.Password + salt.Trim()).ToMD5(),
                        Birthday = customer.Birthday,
                        GenderId = customer.GenderId,
                        Address = customer.Address,
                        //Active = true,
                        Salt = salt,
                        CreateDate = DateTime.Now,
                       

                    };


                    _context.Add(khachhang);
                    await _context.SaveChangesAsync();
                    
                    _notifyService.Success("Tạo mới thành công!");
                }
                else
                {
                    ViewBag.error = "Số điện thoại đã được sử dụng";
                    return View(customer);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenderId"] = new SelectList(_context.Genders, "GenderId", "GenderId", customer.GenderId);
            ViewData["GioiTinh"] = new SelectList(_context.Genders, "GenderId", "GenderName");
            return View(customer);
        }

        // GET: Admin/AdminCustomers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["GioiTinh"] = new SelectList(_context.Genders, "GenderId", "GenderName");
            ViewData["GenderId"] = new SelectList(_context.Genders, "GenderId", "GenderId", customer.GenderId);
            return View(customer);
        }

        // POST: Admin/AdminCustomers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,GenderId,FullName,Birthday,Password,Salt,Address,Email,Phone,CreateDate")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật thành công!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        _notifyService.Error("Có xảy ra lỗi!");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["GioiTinh"] = new SelectList(_context.Genders, "GenderId", "GenderName");
            ViewData["GenderId"] = new SelectList(_context.Genders, "GenderId", "GenderId", customer.GenderId);
            return View(customer);
        }

        // GET: Admin/AdminCustomers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Gender)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/AdminCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            _notifyService.Error("Xóa khách hàng thành công!");
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
