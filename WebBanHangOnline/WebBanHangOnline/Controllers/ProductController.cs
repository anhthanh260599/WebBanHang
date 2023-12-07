using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
using WebBanHangOnline.Models.MySingleton;

namespace WebBanHangOnline.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Product
        public ActionResult Index()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            if (user != null)
            {
                ViewBag.User = user;
            }

            var storeList = db.Stores.ToList().Select(s => new
            {
                Id = s.Id,
                DisplayName = $"{s.Name} -- {s.Address}, {s.District}, {s.Ward}, {s.Province}",
                Address = $"{s.Address}, {s.District}, {s.Ward}, {s.Province}"
            });

            ViewBag.StoreAddress = storeList;

            ViewBag.StoreList = new SelectList(storeList, "Id", "DisplayName");

            var items = db.Products.Where(x=>x.IsActive).ToList();
            return View(items);

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
                items = items.Where(x => x.ProductCategory.Id == id && x.IsActive).ToList();
            }
            var cate = db.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }
            ViewBag.CateId = id;
            return View(items);
        }

        public ActionResult Partial_SanPhamGoiYTheoDanhMuc(int categoryId, int productID)
        {
            var items = db.Products.ToList();
            if (categoryId > 0)
            {
                items = items.Where(x => x.ProductCategory.Id == categoryId && x.IsActive && x.Id != productID).OrderByDescending(x=>x.ViewCount).Take(5).ToList();
            }
            return PartialView(items);
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

        public ActionResult Partial_ProductHot()
        {
            var item = db.Products.Where(x => x.IsHot && x.IsActive).ToList();
            return PartialView(item);
        }

        // Hàm ứng dụng singleton
        [HttpPost]
        public ActionResult UpdateStoreId(int newStoreId)
        {
            // Cập nhật StoreSingleton với giá trị mới
            StoreSingleton.Instance.Id = newStoreId;

            return Json(new { success = true, dataID = StoreSingleton.Instance.Id });
        }

    }
}