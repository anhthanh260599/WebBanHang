using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class CustomerRequestController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/CustomerRequest
        public ActionResult Index()
        {
            var items = db.CustomerRequests.OrderByDescending(x=>x.Id).ToList();
            return View(items);
        }

        public ActionResult Detail(int id)
        {
            var item = db.CustomerRequests.Find(id);
            return View(item);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var item = db.CustomerRequests.Find(id);
            if (item != null)
            {
                db.CustomerRequests.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


        [HttpPost]
        public ActionResult IsResolve(int id)
        {

            var item = db.CustomerRequests.Find(id);
            if (item != null)
            {
                item.IsResolve = !item.IsResolve;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsResolve = item.IsResolve });
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
                        var obj = db.CustomerRequests.Find(Convert.ToInt32(item));
                        db.CustomerRequests.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}