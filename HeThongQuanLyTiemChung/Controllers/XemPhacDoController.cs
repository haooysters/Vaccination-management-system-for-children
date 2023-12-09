using AspNetCoreHero.ToastNotification.Abstractions;
using HeThongQuanLyTiemChung.Helpper;
using HeThongQuanLyTiemChung.Models;
using HeThongQuanLyTiemChung.ModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.Controllers
{
    public class XemPhacDoController : Controller
    {
        private readonly db_VaccineContext _context;

        public INotyfService _notifyService { get; }

        public XemPhacDoController(db_VaccineContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        [Route("xem-phac-do.html", Name = "XemPhacDo")]
        public IActionResult Index(int page = 1)
        {
            var pageNumber = page;
            var pageSize = 5;

            var taikhoanID = HttpContext.Session.GetInt32("CustomerId");
            var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));


            {

                //luu session makh


                List<PhacDo> phacDos = new List<PhacDo>();

                var lsBenh = _context.Diseases.Include(p => p.Injections).ToList();
                var lsMui = _context.Injections.Include(p => p.Disease).Include(p => p.OrderDetails).ToList();

                var checkKHDaTiem = _context.OrderDetails
                    .Where(n => n.Order.CustomerId == khachhang.CustomerId).FirstOrDefault();

                var dsCTVaccine = _context.OrderDetails
                    .Include(p => p.Order)
                    .Include(p => p.Injection)
                    .Include(p => p.Vaccine)
                    .Include(p => p.Injection.Disease)
                    .Where(n => n.Order.CustomerId == khachhang.CustomerId).ToList();





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
                            ThanhTuoi = item.MonthAgeName
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
                                    ThanhTuoi = item.MonthAgeName

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
                                        ThanhTuoi = item.MonthAgeName
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
                        .Where(p => p.CustomerId == khachhang.CustomerId).OrderBy(item => item.AppointmentDate).ToList();

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


                var muihen = _context.AppointDates
                    .Include(p => p.Injection)
                    .Include(p => p.Injection.Disease)
                    .Where(p => p.CustomerId == khachhang.CustomerId && p.AppointmentDate >= DateTime.Now).ToList();


                var dsphacdotheothang = phacDos.OrderBy(p => p.ThanhTuoi);

                ViewBag.Benh = phacDos;

                PagedList<PhacDo> models = new PagedList<PhacDo>(phacDos.AsQueryable(), pageNumber, pageSize);

                ViewBag.Listphacdo = phacDos;

                ViewBag.CurrentPage = pageNumber;

                ViewBag.Customer = khachhang;

                ViewBag.AppountDate = muihen;

                ViewBag.DSPhacDoTheoThang = dsphacdotheothang;


                return View(phacDos);

            }
        }
    }
}
