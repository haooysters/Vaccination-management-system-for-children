using AspNetCoreHero.ToastNotification.Abstractions;
using HeThongQuanLyTiemChung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.Controllers
{
    public class BlogController : Controller
    {
        private readonly db_VaccineContext _context;

        public INotyfService _notifyService { get; }


        public BlogController(db_VaccineContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }
        [Route("blogs.html", Name = ("Page"))]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 2;
            var lsTinDangs = _context.Pages
                .AsNoTracking()
                .OrderByDescending(x => x.PageId);
            PagedList<Page> models = new PagedList<Page>(lsTinDangs, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        [Route("/tin-tuc/{Title}-{id}.html", Name = "PageDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var tindang = _context.Pages.FirstOrDefault(x => x.PageId == id);

                if (tindang == null)
                {
                    return RedirectToAction("Index");
                }
                return View(tindang);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
