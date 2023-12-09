using AspNetCoreHero.ToastNotification.Abstractions;
using HeThongQuanLyTiemChung.Extension;
using HeThongQuanLyTiemChung.Helpper;
using HeThongQuanLyTiemChung.Models;
using HeThongQuanLyTiemChung.ModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly db_VaccineContext _context;

        public INotyfService _notifyService { get; }

        public SearchController(db_VaccineContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }


        public IActionResult TimKiem(string sKey, int txtCatID, int page = 1)
        {
            var pageNumber = page;
            var pageSize = 4;

            string tukhoa = sKey;



            if (sKey != null)
            {
                // tim kiem khach hang the sdt
                var lsTKH = _context.Customers.Include(p => p.Gender).Where(n => n.Phone == sKey).FirstOrDefault();



                if (lsTKH != null)
                {

                    //luu session makh
                    HttpContext.Session.SetInt32("KhachHang", lsTKH.CustomerId);

                    List<PhacDo> phacDos = new List<PhacDo>();

                    var lsBenh = _context.Diseases.Include(p => p.Injections).ToList();
                    var lsMui = _context.Injections.Include(p => p.Disease).Include(p => p.OrderDetails).ToList();

                    var checkKHDaTiem = _context.OrderDetails
                        .Where(n => n.Order.CustomerId == lsTKH.CustomerId).FirstOrDefault();

                    var dsCTVaccine = _context.OrderDetails
                        .Include(p => p.Order)
                        .Include(p => p.Injection)
                        .Include(p => p.Vaccine)
                        .Include(p => p.Injection.Disease)
                        .Where(n => n.Order.CustomerId == lsTKH.CustomerId).ToList();





                    foreach (var item in lsMui)
                    {
                        if (checkKHDaTiem == null)
                        {
                            PhacDo dotTiem = new PhacDo
                            {
                                InjectionId = item.InjectionId,
                                DiseaseId = item.DiseaseId,
                                DiseaseName = item.Disease.DiseaseName,
                                InjectionName = item.InjectionName,
                                MonthAgeName = Utilities.GetMonthOfAge(item.MonthAgeName),

                            };
                            phacDos.Add(dotTiem);
                        }
                        else
                        {
                            foreach (var vc in dsCTVaccine)
                            {
                                //var muitiem = phacDos.Where(n => n.InjectionId == item.InjectionId).FirstOrDefault(); ;

                                //if (muitiem == null)
                                //{

                                if (item.InjectionId == vc.InjectionId)
                                {
                                    PhacDo dotTiem = new PhacDo
                                    {
                                        InjectionId = item.InjectionId,
                                        DiseaseId = item.DiseaseId,
                                        DiseaseName = item.Disease.DiseaseName,
                                        InjectionName = item.InjectionName,
                                        MonthAgeName = Utilities.GetMonthOfAge(item.MonthAgeName),
                                        CreateDate = vc.Order.CreateDate,
                                        VaccineName = vc.Vaccine.VaccineName,
                                        SoLo = vc.LotNumber,
                                        LieuLuong = vc.Dosage

                                    };
                                    phacDos.Add(dotTiem);
                                }
                                else
                                {
                                    var muitiem = phacDos.Where(n => n.InjectionId == item.InjectionId).FirstOrDefault(); ;

                                    if (muitiem == null)
                                    {
                                        PhacDo dotTiem = new PhacDo
                                        {
                                            InjectionId = item.InjectionId,
                                            DiseaseId = item.DiseaseId,
                                            DiseaseName = item.Disease.DiseaseName,
                                            InjectionName = item.InjectionName,
                                            MonthAgeName = Utilities.GetMonthOfAge(item.MonthAgeName),

                                        };
                                        phacDos.Add(dotTiem);
                                    }
                                }
                            }

                        }
                    }

                    // var dsvaccinecungngay = dsCTVaccine.Where(p => phacDos.Any(db => db.DiseaseId == p.Injection.DiseaseId && db.CreateDate == p.Order.CreateDate || db.CreateDate == null)).ToList();


                    foreach (var item in phacDos.ToList())
                    {
                        foreach (var vc in phacDos.ToList())
                        {
                            if (item.DiseaseId == vc.DiseaseId && item.InjectionId == vc.InjectionId && item.CreateDate != null && vc.CreateDate == null)
                            {

                                phacDos.Remove(vc);

                            }

                        }

                    }


                    var dsngayhen = _context.AppointDates.Include(p => p.Customer).Include(p => p.Injection)
                        .Where(p => p.CustomerId == lsTKH.CustomerId).OrderBy(item => item.AppointmentDate).ToList();

                    if (dsngayhen != null)
                    {
                        foreach (var item in phacDos.ToList())
                        {
                            foreach (var ds in dsngayhen.ToList())
                            {
                                if (item.InjectionId == ds.InjectionId)
                                {
                                    item.NgayHen = ds.AppointmentDate;

                                }
                            }
                        }
                    }

                    //Loc phac do theo benh
                    List<PhacDo> dsphacdo = new List<PhacDo>();

                    if (txtCatID != 0)
                    {
                        dsphacdo = phacDos.Where(p => p.DiseaseId == txtCatID).ToList();
                    }
                    else
                    {
                        dsphacdo = phacDos.ToList();
                    }


                    ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");

                    ViewBag.Benh = phacDos;

                    PagedList<PhacDo> models = new PagedList<PhacDo>(dsphacdo.AsQueryable(), pageNumber, pageSize);

                    ViewBag.Listphacdo = phacDos;

                    ViewBag.CurrentPage = pageNumber;

                    ViewBag.tukhoa = tukhoa;

                    ViewBag.KhachHang = lsTKH;

                    return View(models);

                }
                else
                {
                    _notifyService.Error("Không tìm thấy thông tin khách hàng nào!");
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                var taikhoanID = HttpContext.Session.GetInt32("KhachHang");
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));

                // tim kiem khach hang the sdt
                var lsTKH = _context.Customers.Include(p => p.Gender).Where(n => n.Phone == khachhang.Phone).FirstOrDefault();

                if (lsTKH != null)
                {

                    //luu session makh
                    HttpContext.Session.SetInt32("KhachHang", lsTKH.CustomerId);

                    List<PhacDo> phacDos = new List<PhacDo>();

                    var lsBenh = _context.Diseases.Include(p => p.Injections).ToList();
                    var lsMui = _context.Injections.Include(p => p.Disease).Include(p => p.OrderDetails).ToList();

                    var checkKHDaTiem = _context.OrderDetails
                        .Where(n => n.Order.CustomerId == lsTKH.CustomerId).FirstOrDefault();

                    var dsCTVaccine = _context.OrderDetails
                        .Include(p => p.Order)
                        .Include(p => p.Injection)
                        .Include(p => p.Vaccine)
                        .Include(p => p.Injection.Disease)
                        .Where(n => n.Order.CustomerId == lsTKH.CustomerId).ToList();





                    foreach (var item in lsMui)
                    {
                        if (checkKHDaTiem == null)
                        {
                            PhacDo dotTiem = new PhacDo
                            {
                                InjectionId = item.InjectionId,
                                DiseaseId = item.DiseaseId,
                                DiseaseName = item.Disease.DiseaseName,
                                InjectionName = item.InjectionName,
                                MonthAgeName = Utilities.GetMonthOfAge(item.MonthAgeName),


                            };
                            phacDos.Add(dotTiem);
                        }
                        else
                        {
                            foreach (var vc in dsCTVaccine)
                            {
                                //var muitiem = phacDos.Where(n => n.InjectionId == item.InjectionId).FirstOrDefault(); ;

                                //if (muitiem == null)
                                //{

                                if (item.InjectionId == vc.InjectionId)
                                {
                                    PhacDo dotTiem = new PhacDo
                                    {
                                        InjectionId = item.InjectionId,
                                        DiseaseId = item.DiseaseId,
                                        DiseaseName = item.Disease.DiseaseName,
                                        InjectionName = item.InjectionName,
                                        MonthAgeName = Utilities.GetMonthOfAge(item.MonthAgeName),
                                        CreateDate = vc.Order.CreateDate,
                                        VaccineName = vc.Vaccine.VaccineName,
                                        SoLo = vc.LotNumber,
                                        LieuLuong = vc.Dosage,


                                    };
                                    phacDos.Add(dotTiem);
                                }
                                else
                                {
                                    var muitiem = phacDos.Where(n => n.InjectionId == item.InjectionId).FirstOrDefault(); ;

                                    if (muitiem == null)
                                    {
                                        PhacDo dotTiem = new PhacDo
                                        {
                                            InjectionId = item.InjectionId,
                                            DiseaseId = item.DiseaseId,
                                            DiseaseName = item.Disease.DiseaseName,
                                            InjectionName = item.InjectionName,
                                            MonthAgeName = Utilities.GetMonthOfAge(item.MonthAgeName),


                                        };
                                        phacDos.Add(dotTiem);
                                    }
                                }
                            }

                        }
                    }

                    // var dsvaccinecungngay = dsCTVaccine.Where(p => phacDos.Any(db => db.DiseaseId == p.Injection.DiseaseId && db.CreateDate == p.Order.CreateDate || db.CreateDate == null)).ToList();


                    foreach (var item in phacDos.ToList())
                    {
                        foreach (var vc in phacDos.ToList())
                        {
                            if (item.DiseaseId == vc.DiseaseId && item.InjectionId == vc.InjectionId && item.CreateDate != null && vc.CreateDate == null)
                            {

                                phacDos.Remove(vc);

                            }

                        }

                    }


                    var dsngayhen = _context.AppointDates.Include(p => p.Customer).Include(p => p.Injection)
                         .Where(p => p.CustomerId == lsTKH.CustomerId).OrderBy(item => item.AppointmentDate).ToList();

                    if (dsngayhen != null)
                    {
                        foreach (var item in phacDos.ToList())
                        {
                            foreach (var ds in dsngayhen.ToList())
                            {
                                if (item.InjectionId == ds.InjectionId)
                                {
                                    item.NgayHen = ds.AppointmentDate;

                                }
                            }
                        }
                    }


                    //Loc phac do theo benh
                    List<PhacDo> dsphacdo = new List<PhacDo>();

                    if (txtCatID != 0)
                    {
                        dsphacdo = phacDos.Where(p => p.DiseaseId == txtCatID).ToList();
                    }
                    else
                    {
                        dsphacdo = phacDos.ToList();
                    }


                    ViewData["LoaiBenh"] = new SelectList(_context.Diseases, "DiseaseId", "DiseaseName");

                    ViewBag.Benh = phacDos;

                    PagedList<PhacDo> models = new PagedList<PhacDo>(dsphacdo.AsQueryable(), pageNumber, pageSize);

                    ViewBag.Listphacdo = phacDos;

                    ViewBag.CurrentPage = pageNumber;

                    ViewBag.tukhoa = tukhoa;

                    ViewBag.KhachHang = khachhang;

                    return View(models);

                }
                else
                {
                    _notifyService.Error("Không tìm thấy thông tin khách hàng nào!");
                    return RedirectToAction("Index", "Home");
                }
            }


        }

    }
}
