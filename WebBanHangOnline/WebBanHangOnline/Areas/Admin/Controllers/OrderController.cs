using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.Common;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Order
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            var stores = db.Stores.ToList();

            // Chuyển danh sách cửa hàng vào ViewBag hoặc ViewData
            ViewBag.Stores = new SelectList(stores, "Id", "Name");

            if (User.IsInRole("Admin") || User.IsInRole("Coordinator"))
            {
                // Nếu người dùng có vai trò "Admin" hoặc "Người điều phối đơn", hiển thị toàn bộ danh sách Order
                var items = db.Orders.OrderByDescending(x => x.CreateDate).ToList();
                return View(items);
            }
            else if (User.IsInRole("Store") && currentUser != null)
            {
                // Nếu người dùng có vai trò "Store" và đang đăng nhập, lọc danh sách Order có StoreID trùng với UserID của người dùng
                var storeOrders = db.Orders.Where(x => x.Store.UserID == currentUser.Id).OrderByDescending(x => x.CreateDate).ToList();
                return View(storeOrders);
            }

            // Mặc định, nếu không thỏa mãn các điều kiện trên, trả về danh sách trống hoặc thông báo lỗi
            return View(new List<Order>());
        }

        public ActionResult Detail(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_Detail(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderID == id).ToList();
            return PartialView(items);
        }

        public ActionResult DieuPhoiDon(int orderID, int storeId)
        {
            var order = db.Orders.Find(orderID);
            if (order != null)
            {
                db.Orders.Attach(order);
                order.StoreID = storeId;
                order.Status = 6;
                db.Entry(order).Property(x => x.StoreID).IsModified = true;
                db.SaveChanges();
                return Json(new { success = true, message = Message.SuccessSaveChange.ToString() });
            }
            return Json(new { success = false, message = Message.FailureSaveChange.ToString() });
        }

        [HttpPost]
        public ActionResult TuChoiDon(int id)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                db.Orders.Attach(item);
                if(item.TypePayment == 1) // Nếu thanh toán COD thì set Status = 1
                {
                    item.Status = 1;
                }
                else // còn lại thì Status = 2 (do thanh toán online)
                {
                    item.Status = 2;
                }
                item.StoreID = null;
                db.Entry(item).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
                return Json(new { success = true, message = Message.SuccessSaveChange.ToString() });
            }
            return Json(new { success = false, message = Message.FailureSaveChange.ToString() });
        }

        [HttpPost]
        public ActionResult UpdateTrangThai(int id, int trangThai)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                db.Orders.Attach(item);
                item.Status = trangThai;
                if (item.TotalAmount >= 50000 && item.Status == 3) // Status = 3 là giao hàng thành công
                {
                    var user = db.Users.Where(x => x.Id == item.CustomerID).FirstOrDefault();
                    db.Users.Where(x => x.Id == item.CustomerID).FirstOrDefault().CheckPoint++;
                    db.SaveChanges();
                }
                db.Entry(item).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
                return Json(new { success = true, message = Message.SuccessSaveChange.ToString() });

            }
            return Json(new { success = false, message = Message.FailureSaveChange.ToString() });
        }

        public ActionResult CoordinationDetail(int id)
        {

            ViewBag.TotalProductOrder = db.OrderDetails.Where(x => x.OrderID == id).Count();

            List<RecipeDetail> recipeDetails = new List<RecipeDetail>();
            List<Store> listStore = new List<Store>();
            var itemsOrderDetails = db.OrderDetails.Where(x => x.OrderID == id).ToList();

            // Lấy ra danh sách nguyên vật liệu
            var totalMaterials = db.Matterials.ToList();

            // Lấy ra danh sách nvl cần dùng recipeDetails
            var items = db.OrderDetails.Where(x => x.OrderID == id).ToList();
            foreach (OrderDetail orderDetail in items)
            {
                var itemsProductRecipe = db.RecipeDetails.Where(s => s.RecipeProductID == orderDetail.ProductID).ToList();
                if (itemsProductRecipe != null)
                {
                    for (int i = 0; i < itemsProductRecipe.Count; i++)
                    {
                        var itemProductRecipe = itemsProductRecipe[i];
                        recipeDetails.Add(itemProductRecipe);
                    }
                }
            }
            //Lấy ra danh sách cửa hàng có đủ nguyên vật liệu
            foreach (Store store in db.Stores.ToList())
            {
                bool hasEnoughMaterials = true;

                foreach (RecipeDetail recipeDetail in recipeDetails)
                {
                    // Lấy ra nvl 
                    //var storeMaterial = totalMaterials.Where(m => m.Id == recipeDetail.MatterialID && m.StoreID == store.Id).FirstOrDefault();
                    // Kiểm tra số lượng
                    //if (storeMaterial == null || storeMaterial.Quantity < recipeDetail.Quantity)
                    //{
                    //    hasEnoughMaterials = false;
                    //    break;
                    //}
                }
                if (hasEnoughMaterials)
                {
                    listStore.Add(store);
                }
            }

            ViewBag.ListStore = new SelectList(listStore, "Id", "Name");

            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_Detail_Materials(int id)
        {
            List<RecipeDetail> recipeDetails = new List<RecipeDetail>();
            // Lấy ra product id;
            var items = db.OrderDetails.Where(x => x.OrderID == id).ToList();
            foreach (OrderDetail orderDetail in items)
            {
                var itemsProductRecipe = db.RecipeDetails.Where(s => s.RecipeProductID == orderDetail.ProductID).ToList();
                if (itemsProductRecipe != null)
                {
                    for (int i = 0; i < itemsProductRecipe.Count; i++)
                    {
                        var itemProductRecipe = itemsProductRecipe[i];
                        recipeDetails.Add(itemProductRecipe);
                    }

                }
                else
                {
                    ViewBag.ErrorNullRecipe = true;
                    ViewBag.ErrorNullRecipeName = orderDetail.Product.Title;

                    break;
                }

            }

            return PartialView(recipeDetails);
        }
    }
}