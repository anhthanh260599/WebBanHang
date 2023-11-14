using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class MatterialsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Matterials
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId(); // Sử dụng UserManager để lấy UserId của người dùng hiện tại
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = userManager.FindById(currentUserId);

            var items = db.Matterials.OrderByDescending(x => x.Id).ToList();
            if (User.IsInRole("Store"))
            {
                //items = db.Matterials.Where(s => s.Store.UserID == currentUser.Id).ToList();
                items = db.Matterials.Where(x => x.Store.UserID == currentUser.Id).OrderByDescending(x => x.Id).ToList();
            }

            return View(items);
        }

        public ActionResult Add()
        {
            ViewBag.StoreList = new SelectList(db.Stores.ToList(), "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Matterial model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.StoreList = new SelectList(db.Stores.ToList(), "Id", "Name");

                // Lấy thông tin người dùng hiện tại
                var currentUserId = User.Identity.GetUserId(); // Sử dụng UserManager để lấy UserId của người dùng hiện tại

                // Sử dụng DbContext để tìm ApplicationUser có UserId tương ứng
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = userManager.FindById(currentUserId);  

                if (currentUser != null)
                {
                    // Gán giá trị FullName của người dùng hiện tại cho CreateBy của đối tượng News
                    model.CreateBy = currentUser.FullName;
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy người dùng
                    // Chẳng hạn, bạn có thể gán một giá trị mặc định hoặc xử lý khác
                    model.CreateBy = "Người dùng không tồn tại"; // Ví dụ
                }
                model.CreateDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);  // chuyển có dấu thành không dấu, mục đích để làm url sau này
                db.Matterials.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.StoreList = new SelectList(db.Stores.ToList(), "Id", "Name");

            var item = db.Matterials.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Matterial model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.StoreList = new SelectList(db.Stores.ToList(), "Id", "Name");

                db.Matterials.Attach(model);

                // Lấy thông tin người dùng hiện tại
                var currentUserId = User.Identity.GetUserId(); // Sử dụng UserManager để lấy UserId của người dùng hiện tại

                // Sử dụng DbContext để tìm ApplicationUser có UserId tương ứng
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = userManager.FindById(currentUserId);

                if (currentUser != null)
                {
                    // Gán giá trị FullName của người dùng hiện tại cho CreateBy của đối tượng News
                    model.CreateBy = currentUser.FullName;
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy người dùng
                    // Chẳng hạn, bạn có thể gán một giá trị mặc định hoặc xử lý khác
                    model.CreateBy = "Người dùng không tồn tại"; // Ví dụ
                }


                model.ModifierDate = DateTime.Now;
                model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title); // chuyển có dấu thành không dấu, mục đích để làm url sau này
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var item = db.Matterials.Find(id);
            if (item != null)
            {
                db.Matterials.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {

            var item = db.Matterials.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsActive = item.IsActive });
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
                        var obj = db.Matterials.Find(Convert.ToInt32(item));
                        db.Matterials.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}