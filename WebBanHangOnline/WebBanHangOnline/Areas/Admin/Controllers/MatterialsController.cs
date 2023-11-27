using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
using System.Diagnostics;

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
            var quantity = db.Storages.OrderByDescending(x => x.Id).ToList();
            var store = db.Stores.Where(x => x.UserID == currentUser.Id).OrderByDescending(x => x.Id).ToList();
            var currentStore = store.FirstOrDefault();
            List<MaterialQuantityViewModel> viewModel = new List<MaterialQuantityViewModel>();
            if (User.IsInRole("Admin"))
            {
                quantity = db.Storages.Where(x => x.StoreId == 0).OrderByDescending(x => x.Id).ToList();
            }
            if (User.IsInRole("Store"))
            {
                //items = db.Matterials.Where(s => s.Store.UserID == currentUser.Id).ToList();
                quantity = db.Storages.Where(x => x.StoreId == currentStore.Id).OrderByDescending(x => x.Id).ToList();
            }
            for (int i = 0; i < quantity.Count; i++)
            {
                var materialId = quantity[i].MaterialID;
                var itemMaterial = db.Matterials.Where(x => x.Id == materialId).ToList();
                var viewModel1 = new MaterialQuantityViewModel(itemMaterial[0], quantity[i]);
                viewModel.Add(viewModel1);
            }
            return View(viewModel);
        }

        public ActionResult Add()
        {
            ViewBag.StoreList = new SelectList(db.Stores.ToList(), "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(MaterialQuantityViewModel model)
        {
            Matterial insert = new Matterial();
            insert.Title = model.Title;
            insert.MattsCode = model.MattsCode;
            insert.Description = model.Description;
            insert.Price = model.Price;
            insert.Image = model.Image;
            insert.Packing = model.Packing;
            insert.Detail = model.Detail;
            insert.IsActive = model.IsActive;
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
                    insert.CreateBy = currentUser.FullName;
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy người dùng
                    // Chẳng hạn, bạn có thể gán một giá trị mặc định hoặc xử lý khác
                    insert.CreateBy = "Người dùng không tồn tại"; // Ví dụ
                }
                insert.SetCreated();
                insert.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(insert.Title);  // chuyển có dấu thành không dấu, mục đích để làm url sau này
                db.Matterials.Add(insert);
                db.SaveChanges();
                var newest = db.Matterials.OrderByDescending(m => m.Id).FirstOrDefault();
                int tempID = newest.Id;
                Storage storage = new Storage();
                storage.StoreId = 0;
                storage.MaterialID = tempID;
                storage.Quantity = model.Quantity;
                db.Storages.Add(storage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.StoreList = new SelectList(db.Stores.ToList(), "Id", "Name");

            var item = db.Matterials.Find(id);
            
            //Lấy ra cửa hàng
            var currentUserId = User.Identity.GetUserId(); // Sử dụng UserManager để lấy UserId của người dùng hiện tại
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = userManager.FindById(currentUserId);
            var quantity = db.Storages.OrderByDescending(x => x.Id).ToList();
            var store = db.Stores.Where(x => x.UserID == currentUser.Id).OrderByDescending(x => x.Id).ToList();
            var currentStore = store.FirstOrDefault();
            if (User.IsInRole("Admin"))
            {
                quantity = db.Storages.Where(x => x.StoreId == 0).OrderByDescending(x => x.Id).ToList();
            }
            if (User.IsInRole("Store"))
            {
                //items = db.Matterials.Where(s => s.Store.UserID == currentUser.Id).ToList();
                quantity = db.Storages.Where(x => x.StoreId == currentStore.Id).OrderByDescending(x => x.Id).ToList();
            }
            var storage = quantity.Where(x => x.MaterialID == id).ToList();
            MaterialQuantityViewModel model = new MaterialQuantityViewModel(item, storage[0]);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MaterialQuantityViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.StoreList = new SelectList(db.Stores.ToList(), "Id", "Name");

                var edit = db.Matterials.Find(model.Id);
                edit.Title = model.Title;
                edit.MattsCode = model.MattsCode;
                edit.Description = model.Description;
                edit.Price = model.Price;
                edit.Image = model.Image;
                edit.Packing = model.Packing;
                edit.Detail = model.Detail;
                edit.IsActive = model.IsActive;

                db.Matterials.Attach(edit);

                // Lấy thông tin người dùng hiện tại
                var currentUserId = User.Identity.GetUserId(); // Sử dụng UserManager để lấy UserId của người dùng hiện tại
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = userManager.FindById(currentUserId);

                //Lấy ra cửa hàng
                var quantity = db.Storages.OrderByDescending(x => x.Id).ToList();
                var store = db.Stores.Where(x => x.UserID == currentUser.Id).OrderByDescending(x => x.Id).ToList();
                var currentStore = store.FirstOrDefault();
                var currenrStorage = quantity;
                if (User.IsInRole("Admin"))
                {
                    quantity = db.Storages.Where(x => x.StoreId == 0).OrderByDescending(x => x.Id).ToList();
                    currenrStorage = quantity.Where(x => x.MaterialID == model.Id).ToList();
                }
                if (User.IsInRole("Store"))
                {
                    //items = db.Matterials.Where(s => s.Store.UserID == currentUser.Id).ToList();
                    quantity = db.Storages.Where(x => x.StoreId == currentStore.Id).OrderByDescending(x => x.Id).ToList();
                    currenrStorage = quantity.Where(x => x.MaterialID == model.Id).ToList();
                }
                var update = currenrStorage[0];
                int num = model.Quantity;
                update.Quantity = num;

                if (currentUser != null)
                {
                    // Gán giá trị FullName của người dùng hiện tại cho CreateBy của đối tượng News
                    edit.CreateBy = currentUser.FullName;
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy người dùng
                    // Chẳng hạn, bạn có thể gán một giá trị mặc định hoặc xử lý khác
                    edit.CreateBy = "Người dùng không tồn tại"; // Ví dụ
                }
                edit.SetModified();
                edit.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(edit.Title); // chuyển có dấu thành không dấu, mục đích để làm url sau này
                db.Entry(edit).State = System.Data.Entity.EntityState.Modified;
                db.Entry(update).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var item = db.Matterials.Find(id);
            var storage = db.Storages.Where(s => s.MaterialID == id).ToList();
            if (item != null)
            {
                db.Matterials.Remove(item);
                for(int i = 0; i < storage.Count; i++)
                    db.Storages.Remove(storage[i]);
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
                        var storage = db.Storages.Where(s => s.MaterialID == obj.Id).ToList();
                        db.Matterials.Remove(obj);
                        for (int i = 0; i < storage.Count; i++)
                            db.Storages.Remove(storage[i]);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}