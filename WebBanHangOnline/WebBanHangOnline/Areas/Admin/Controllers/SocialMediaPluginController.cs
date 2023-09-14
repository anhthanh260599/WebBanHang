using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class SocialMediaPluginController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/SocialMediaPlugin
        public ActionResult Index()
        {
            var items = db.SocialMediaPlugins.ToList();
            return View(items);
        }


        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(SocialMediaPlugin model)
        {
            if (ModelState.IsValid)
            {
                db.SocialMediaPlugins.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }


        public ActionResult Edit(int id)
        {
            var item = db.SocialMediaPlugins.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SocialMediaPlugin model)
        {
            if (ModelState.IsValid)
            {
                db.SocialMediaPlugins.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var item = db.SocialMediaPlugins.Find(id);
            if (item != null)
            {
                db.SocialMediaPlugins.Remove(item);
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
                        var obj = db.SocialMediaPlugins.Find(Convert.ToInt32(item));
                        db.SocialMediaPlugins.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}