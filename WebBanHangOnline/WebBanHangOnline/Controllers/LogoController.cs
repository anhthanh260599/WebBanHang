using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Controllers
{
    public class LogoController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Logo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Partial_Logo()
        {
            var item = db.Logos.FirstOrDefault();
            return PartialView(item);
        }
    }
}