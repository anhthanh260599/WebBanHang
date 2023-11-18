using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Products
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            if (User.IsInRole("Admin"))
            {
                // Nếu người dùng có vai trò "Admin" hoặc "Người điều phối đơn", hiển thị toàn bộ danh sách Order
                var items = db.Products.OrderByDescending(x => x.Id).ToList();
                return View(items);
            }
            else if (User.IsInRole("Store") && currentUser != null)
            {
                // Nếu người dùng có vai trò "Store" và đang đăng nhập, lọc danh sách Order có StoreID trùng với UserID của người dùng
                var storeProduct = db.Products.Where(x => x.Store.UserID == currentUser.Id).OrderByDescending(x => x.Id).ToList();
                return View(storeProduct);
            }

            // Mặc định, nếu không thỏa mãn các điều kiện trên, trả về danh sách trống hoặc thông báo lỗi
            return View(new List<Product>());
            //var items = db.Products.OrderByDescending(x => x.Id).ToList();
            //return View(items);
        }

        public ActionResult Add()
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            ViewBag.StoreList = new SelectList(db.Stores.ToList(), "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product model, List<string> Images,List<int> rDefault)
        {
            try
            {
                ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
                ViewBag.StoreList = new SelectList(db.Stores.ToList(), "Id", "Name");

                var userId = User.Identity.GetUserId();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var user = userManager.FindById(userId);

                var currentUser = userManager.FindById(User.Identity.GetUserId());
                var storeID = db.Stores.Where(x=>x.UserID == currentUser.Id).Select(x=>x.Id).FirstOrDefault();

                

                if(model.PriceSale != null && model.IsSale == false)
                {
                    model.IsSale = true;
                }

                if (Images != null && Images.Count > 0)
                {
                    for (int i = 0; i < Images.Count; i++)
                    {
                        if (i + 1 == rDefault[0])
                        {
                            model.Image = Images[i];
                            model.ProductImage.Add(new ProductImage
                            {
                                ProductID = model.Id,
                                Image = Images[i],
                                IsDefault = true
                            });
                        }
                        else
                        {
                            model.ProductImage.Add(new ProductImage
                            {
                                ProductID = model.Id,
                                Image = Images[i],
                                IsDefault = false
                            }); ;
                        }

                    }
                }
                else
                {
                    ViewBag.ErrorImage = "Vui lòng không để trống";
                }
                model.CreateDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                if (string.IsNullOrEmpty(model.SeoTitle))
                {
                    model.SeoTitle = model.Title;
                }
                if (string.IsNullOrEmpty(model.Alias))
                {
                    model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);
                }

                if (User.IsInRole("Store")) // Nếu là cửa hàng thêm sản phẩm thì phải đợi Admin duyệt
                {
                    model.IsActive = false;
                    model.IsApprovedByAdmin = false;
                    model.StoreID = storeID;
                }
                else
                {
                    NullProduct newModel = new NullProduct();
                    model.StoreID = newModel.StoreID;
                    model.IsApprovedByAdmin = true; // Nếu là admin thì tự động duyệt
                }

                db.Products.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
                //return Json(new { id = model.Id });
            }
            catch
            {
                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            ViewBag.listProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            ViewBag.StoreList = new SelectList(db.Stores.ToList(), "Id", "Name");
            var item = db.Products.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model)
        {
            ViewBag.listProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            ViewBag.StoreList = new SelectList(db.Stores.ToList(), "Id", "Name");
            var productImage = db.ProductImage.Where(x => x.ProductID == model.Id && x.IsDefault == true).FirstOrDefault();

            var userId = User.Identity.GetUserId();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = userManager.FindById(userId);

            var currentUser = userManager.FindById(User.Identity.GetUserId());
            var storeID = db.Stores.Where(x => x.UserID == currentUser.Id).Select(x => x.Id).FirstOrDefault();

            try
            {
                db.Products.Attach(model);

                if (User.IsInRole("Store")) // Nếu là cửa hàng thêm sản phẩm thì phải đợi Admin duyệt
                {
                    model.StoreID = storeID;
                }

                if (model.PriceSale != null && model.IsSale == false)
                {
                    model.IsSale = true;
                }
           
                model.IsApprovedByAdmin = true;
                model.Image = productImage.Image;
                model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);
                model.ModifierDate = DateTime.Now;
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Products.Find(id);
            if(item != null)
            {
                db.Products.Remove(item);
                db.SaveChanges();
                return Json(new {success = true});
            }
            return Json(new {success = false});
        }

        [HttpPost]
        public ActionResult IsActive(int id) 
        {
            var item = db.Products.Find(id);
            if(item != null)
            {
                item.IsActive =! item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new {success = true , IsActive = item.IsActive});
            }
            return Json(new {success = false});
        }

        [HttpPost]
        public ActionResult IsHome(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsHome = !item.IsHome;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsHome = item.IsHome });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsHot(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsHot = !item.IsHot;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsHot = item.IsHot });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsFeature(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsFeature = !item.IsFeature;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsFeature = item.IsFeature });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsApprove(int id)
        {

            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsApprovedByAdmin = !item.IsApprovedByAdmin;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsApprovedByAdmin = item.IsApprovedByAdmin });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteSelected(string ids)
        {
            if(!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if(items!=null && items.Any())
                {
                    foreach(var item in items)
                    {
                        var obj = db.Products.Find(Convert.ToInt32(item));
                        db.Products.Remove(obj);
                        db.SaveChanges();
                    }
                }   
                return Json(new {success = true});
            }    
            return Json(new {success =false});
        }
    }
}