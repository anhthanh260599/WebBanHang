using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Controllers
{
    public class ArticalController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Artical
        public ActionResult Index(string alias)
        {
            var item = db.Posts.Where(x=>x.IsActive).FirstOrDefault(x=>x.Alias == alias);
            return View(item);
        }
    }
}