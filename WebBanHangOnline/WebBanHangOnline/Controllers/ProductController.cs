using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Product
        public ActionResult Index()
        {
            var items = db.Products.Where(x => x.IsActive).ToList();
            return View(items);
        }

        [HttpPost]
        public ActionResult Index(string dataSearch)
        {
            var items = db.Products.Where(p => p.Title.StartsWith(dataSearch) && p.IsActive).ToList();

            return Json(new { dataList = items });
        }

        [HttpPost]
        public ActionResult SavePageSize(int pageSize)
        {
            Session["pageSize"] = pageSize;
            return Json(new { success = true });
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
            var countReviewProduct = db.ReviewProducts.Where(x=>x.ProductId == id && x.IsApproved).Count();
            ViewBag.ReviewProductCount = countReviewProduct;
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
            var item = db.Products.Where(x=>x.IsFeature && x.IsActive && x.IsHome).Take(10).ToList();
            return PartialView(item);
        }

        public ActionResult Partial_ProductSale()
        {
            var item = db.Products.Where(x=>x.IsSale && x.IsActive).ToList();
            return PartialView(item);
        }

        
    }
}