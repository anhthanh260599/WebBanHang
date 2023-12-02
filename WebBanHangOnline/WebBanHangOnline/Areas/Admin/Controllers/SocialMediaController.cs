using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
using WebBanHangOnline.Models.ServicePattern;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class SocialMediaController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly ISocialMediaService _socialMediaService;
        // GET: Admin/SocialMedia

        public SocialMediaController(ISocialMediaService socialMediaService)
        {
            _socialMediaService = socialMediaService;
        }
        public SocialMediaController() : this(new SocialMediaService(new ApplicationDbContext()))
        {
            // You can provide a default implementation for the service here if needed
        }
        public ActionResult Index()
        {
            //var items = db.SocialMediaProfiles.ToList();
            var items = _socialMediaService.GetAllProfile();
            return View(items);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(SocialMediaProfiles model)
        {
            if (ModelState.IsValid)
            {
                //db.SocialMediaProfiles.Add(model);
                //db.SaveChanges();
                _socialMediaService.AddProfile(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            //var item = db.SocialMediaProfiles.Find(id);
            var item = _socialMediaService.GetProfileById(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SocialMediaProfiles model)
        {
            if (ModelState.IsValid)
            {
                //db.SocialMediaProfiles.Attach(model);
                //db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _socialMediaService.UpdateProfile(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            //var item = db.SocialMediaProfiles.Find(id);
            //if (item != null)
            //{
            //    db.SocialMediaProfiles.Remove(item);
            //    db.SaveChanges();
            //    return Json(new { success = true });
            //}
            _socialMediaService.DeleteProfile(id);
            return Json(new { success = true });
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
                        var obj = db.SocialMediaProfiles.Find(Convert.ToInt32(item));
                        db.SocialMediaProfiles.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}