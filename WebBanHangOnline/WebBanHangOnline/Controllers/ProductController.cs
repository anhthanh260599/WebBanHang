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
        public ActionResult Index(int? id)
        {
            var items = db.Products.ToList();
            return View(items);
        }

        public ActionResult Detail(string alias, int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                db.Products.Attach(item);
                item.ViewCount = item.ViewCount + 1;   // Đếm số lượt xem
                db.Entry(item).Property(x=>x.ViewCount).IsModified = true;
                db.SaveChanges();
            }
            return View(item);
        }

        public ActionResult ProductCategory( string alias, int id)
        {
            var items = db.Products.ToList();

            if (id > 0)
            {
                items = items.Where(x => x.ProductCategory.Id == id).ToList();
            }
            var cate = db.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }
            ViewBag.CateId = id;
            return View(items);
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