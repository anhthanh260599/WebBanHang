using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Controllers
{
    public class StoreController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Store
        public ActionResult Index()
        {
            var items = db.Stores.Where(x=>x.IsActive).ToList();
            return View(items);
        }

        public ActionResult Detail(int id)
        {
            var item = db.Stores.Find(id);
            return View(item);
        }
    }
}