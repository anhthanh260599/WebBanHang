using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Partial_ItemsByCateId()
        {
            var item = db.Products.Where(x=>x.IsHome && x.IsActive).Take(10).ToList();
            return PartialView(item);
        }

        public ActionResult Partial_ProductSale()
        {
            var item = db.Products.Where(x=>x.IsSale && x.IsActive).ToList();
            return PartialView(item);
        }
    }
}