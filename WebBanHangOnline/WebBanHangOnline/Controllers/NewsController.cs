using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

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

        public ActionResult Partial_News_Home()
        {
            var items = db.News.OrderByDescending(x=>x.Id).Where(x=>x.IsActive).Take(3);
            return PartialView(items);
        }
    }
}