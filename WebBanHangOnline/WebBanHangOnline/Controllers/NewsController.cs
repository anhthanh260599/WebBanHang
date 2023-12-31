using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    public class NewsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: News
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData()
        {
            var items = db.News
                .OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    Id = x.Id,
                    Title = x.Title,
                    Alias = x.Alias,
                    Image = x.Image,
                    CreateBy = x.CreateBy,
                    CreateDate = x.CreateDate,
                    Description = x.Description
                })
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SavePageSize(int pageSize)
        {
            Session["pageSize"] = pageSize;
            return Json(new { success = true });
        }


        public ActionResult Detail(int id)
        {
            var item = db.News.Find(id);
            return View(item);
        }

        public ActionResult Partial_News_Home()
        {
            var items = db.News.OrderByDescending(x=>x.Id).Where(x=>x.IsActive).Take(3);
            return PartialView(items);
        }
    }
}