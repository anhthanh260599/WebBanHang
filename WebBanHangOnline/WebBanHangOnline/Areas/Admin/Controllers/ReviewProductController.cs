using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class ReviewProductController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/ReviewProduct
        public ActionResult Index()
        {
            var items = db.ReviewProducts.OrderByDescending(x => x.Id).ToList();
            return View(items);
        }

        public ActionResult Detail(int id)
        {
            var item = db.ReviewProducts.Find(id);
            return View(item);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var item = db.ReviewProducts.Find(id);
            if (item != null)
            {
                db.ReviewProducts.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsApprove(int id)
        {

            var item = db.ReviewProducts.Find(id);
            if (item != null)
            {
                item.IsApproved = !item.IsApproved;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsApproved = item.IsApproved });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteSelected(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.ReviewProducts.Find(Convert.ToInt32(item));
                        db.ReviewProducts.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}