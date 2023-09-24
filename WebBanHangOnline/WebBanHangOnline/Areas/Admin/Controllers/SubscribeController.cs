using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class SubscribeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Subscribe
        public ActionResult Index()
        {
            var items = db.Subscribes.OrderByDescending(x => x.Id).ToList();
            return View(items);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var item = db.Subscribes.Find(id);
            if (item != null)
            {
                db.Subscribes.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
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
                        var obj = db.Subscribes.Find(Convert.ToInt32(item));
                        db.Subscribes.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}