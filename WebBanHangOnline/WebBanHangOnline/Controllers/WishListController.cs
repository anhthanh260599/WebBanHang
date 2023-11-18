using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
using System.Web.Services.Description;
using WebBanHangOnline.Models.Common;


namespace WebBanHangOnline.Controllers
{
    public class WishListController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: WishList
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var items = db.WishLists.Where(x=>x.CustomerID == userId).ToList();
            return View(items);
        }

        [HttpPost]
        public ActionResult PostWishList(int productId)
        {
            var userId = User.Identity.GetUserId();

            if (Request.IsAuthenticated == false) // Kiểm tra đăng nhập
            {
                return Json(new { success = false, message = Models.Common.Message.PleaseLogin.ToString() });

            }

            var item = new WishList();
            item.ProductId = productId;
            item.CustomerID = User.Identity.GetUserId();
            item.CustomerName = User.Identity.GetUserName();
            item.CreatedDate = DateTime.Now;
            db.WishLists.Add(item);
            db.SaveChanges();
            return Json(new {success = true, message = Models.Common.Message.SuccessAddWishList.ToString() });
        }

        [HttpPost]
        public ActionResult PostDeleteWishList(int productId)
        {
            var userId = User.Identity.GetUserId();
            var checkItem = db.WishLists.FirstOrDefault(x => x.ProductId == productId && x.CustomerID == userId);
            if (checkItem != null)
            {
                db.WishLists.Remove(checkItem);
                db.SaveChanges();
                return Json(new { success = true, message = Models.Common.Message.SuccessDeleted.ToString() });
            }
            return Json(new { success = false, message = Models.Common.Message.FailDeleted.ToString() });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var item = db.WishLists.Find(id);
            if (item != null)
            {
                db.WishLists.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}