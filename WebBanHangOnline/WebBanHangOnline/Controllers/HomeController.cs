using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.Common;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Partial_HomeSlider()
        {
            var items = db.Sliders.ToList();
            return PartialView(items);
        }

        public ActionResult Partial_NewItem()
        {
            var item = db.Posts.Where(x=>x.Alias == "san-pham-moi" && x.IsActive).FirstOrDefault();
            return PartialView(item);
        }

        public ActionResult Partial_Subscribe()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subscribe(Subscribe request)
        {
            if (ModelState.IsValid)
            {
                db.Subscribes.Add(new Subscribe
                {
                    Email = request.Email,
                    CreateDate = DateTime.Now,
                });
                db.SaveChanges();
                return Json(new {success = true});
            }
            return View("Partial_Subscribe",request);
        }

        // Hàm dùng để thống kê số lượt truy cập
        public ActionResult Refresh()
        {
            var item = new ThongKeViewModelString();

            ViewBag.Visitors_online = HttpContext.Application["visitors_online"];

            item.HomNay = HttpContext.Application["HomNay"].ToString();
            item.HomQua =  HttpContext.Application["HomQua"].ToString();
            item.TuanNay = HttpContext.Application["TuanNay"].ToString();
            item.TuanTruoc = HttpContext.Application["TuanTruoc"].ToString();
            item.ThangNay = HttpContext.Application["ThangNay"].ToString();
            item.ThangTruoc = HttpContext.Application["ThangTruoc"].ToString();
            item.TatCa = HttpContext.Application["TatCa"].ToString();
            return PartialView(item);
        }
    }
}