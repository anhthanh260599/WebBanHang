using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
using System.Data.Entity;
using WebBanHangOnline.Models.Common;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class StoreOrderController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/StoreOrder

        public ActionResult Index()
        {
            var items = db.Matterials.ToList();
            return View(items);
        }

        [HttpPost]

        public ActionResult Index(List<OrderDetailMatts> list, decimal total)
        {
            var admin = db.Storages.Where(x => x.StoreId == 0).OrderByDescending(x => x.Id).ToList();
            var temp = admin;
            int matID;
            for(int i =0; i<list.Count; i++)
            {
                matID = list[i].MatterialID;
                temp = admin.Where(x => x.MaterialID == matID).ToList();
                if (temp[0].Quantity < list[i].Quantity)
                {
                    ViewBag.NotEnought = $"Không có đủ số lượng cho {list[i].Matterial.Title}";
                    return View(list);
                }
            }

            // Them orderMatts vao db
            OrderMatts orderMatts = new OrderMatts();
            var currentUserId = User.Identity.GetUserId(); // Sử dụng UserManager để lấy UserId của người dùng hiện tại
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = userManager.FindById(currentUserId);

            if (currentUser != null)
            {
                // Gán giá trị FullName của người dùng hiện tại cho CreateBy của đối tượng News
                orderMatts.CreateBy = currentUser.FullName;
            }
            else
            {
                // Xử lý trường hợp không tìm thấy người dùng
                // Chẳng hạn, bạn có thể gán một giá trị mặc định hoặc xử lý khác
                orderMatts.CreateBy = "Người dùng không tồn tại"; // Ví dụ
            }

            var store = db.Stores.Where(x => x.Name == currentUser.FullName).FirstOrDefault();

            if (store != null)
            {
                orderMatts.StoreID = store.Id;
            }
            else if (store == null) 
            {
                orderMatts.StoreID = 14;
            }

            ////////// Tạo mã đơn hàng với Ngày/Tháng/Năm //////////////
            // Lấy ngày hiện tại dưới dạng ddMMyy
            string currentDate = DateTime.Now.ToString("ddMMyy");
            // Lấy ra tất cả các mã đơn hàng trong ngày hiện tại
            var orderCodes = db.OrderMatts
                .Where(o => o.Code.StartsWith("DHNVL" + currentDate))
                .Select(o => o.Code)
                .ToList();
            // Tìm số đơn hàng cuối cùng trong ngày
            int lastOrderNumber = 0;
            foreach (var newCode in orderCodes)
            {
                int orderNumber;
                if (int.TryParse(newCode.Substring(11), out orderNumber))
                {
                    if (orderNumber > lastOrderNumber)
                    {
                        lastOrderNumber = orderNumber;
                    }
                }
            }
            // Tạo mã đơn hàng mới
            string newOrderCode = $"DHNVL{currentDate}{(lastOrderNumber + 1).ToString("D5")}"; // D5 có nghĩa là 5 số 0 cuối, rồi + lên
                                                                                            ////////// End Tạo mã đơn hàng với Ngày/Tháng/Năm //////////////


            // Gán mã đơn hàng và lưu vào cơ sở dữ liệu
            orderMatts.Code = newOrderCode;


            //orderMatts.Code = "Test";
            orderMatts.CustomerName = currentUser.FullName;
            orderMatts.Phone = currentUser.Phone;
            orderMatts.Email = currentUser.Email;
            if (store != null)
            {
                orderMatts.Address = store.Address + ", " + store.Ward + ", " + store.District + ", " + store.Province;
            }
            else if (store == null)
            {
                orderMatts.Address = "Vui lòng liên hệ cửa hàng";
            }

            orderMatts.TotalAmount = total;
            orderMatts.Status = 1;

            orderMatts.CreateDate = DateTime.Now;
            orderMatts.ModifierDate = DateTime.Now;

            using (var context = new ApplicationDbContext())
            {
                context.OrderMatts.Add(orderMatts);
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].OrderMattsID = orderMatts.Id;
                    context.OrderDetailMatts.Add(list[i]);
                }
                context.SaveChanges();
            }

            return Json(new { newUrl = Url.Action("OrderIndex", "StoreOrder") }); // Truyền đến quản lý đơn nvl
        }

        public ActionResult OrderIndex()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            var items = db.OrderMatts.OrderByDescending(x => x.Id).ToList();
            if (User.IsInRole("Admin"))
            {
                // Nếu người dùng có vai trò "Admin" hoặc "Người điều phối đơn", hiển thị toàn bộ danh sách Order
                items = db.OrderMatts.OrderByDescending(x => x.Id).ToList();
                return View(items);
            }
            else if (User.IsInRole("Store") && currentUser != null)
            {
                // Nếu người dùng có vai trò "Store" và đang đăng nhập, lọc danh sách Order có StoreID trùng với UserID của người dùng
                var storeProduct = db.OrderMatts.Where(x => x.Store.UserID == currentUser.Id).OrderByDescending(x => x.Id).ToList();
                return View(storeProduct);
            }
            return View(items);
        }

        public ActionResult OrderDetail(int id)
        {
            var item = db.OrderMatts.Find(id);
            return View(item);
        }

        public ActionResult Partial_OrderDetail(int id)
        {
            var items = db.OrderDetailMatts.Where(x => x.OrderMattsID == id).ToList();
            return PartialView(items);
        }


        [HttpPost]
        public ActionResult UpdateTrangThai(int id, int trangThai, int storeId)
        {
            if(trangThai == 3)
            {
                var detail = db.OrderDetailMatts.Where(x => x.OrderMattsID == id).ToList();
                int quantity = 0;
                int material = 0;
                //Lấy storege của cửa hàng
                var storage = db.Storages.Where(x => x.StoreId == storeId).ToList();
                var currentStorage = storage.ToList();
                //Lấy storage admin
                var admin = db.Storages.Where(x => x.StoreId == 0).ToList();
                var currentAdmin = admin.ToList();
                for (int i = 0; i < detail.Count; i++)
                {
                    material = detail[i].MatterialID;
                    quantity = detail[i].Quantity;
                    //Lấy storage của mat hiện tại
                    currentStorage = storage.Where(x => x.MaterialID == material).ToList();
                    //Lấy storage admin của mat hiện tại
                    currentAdmin = admin.Where(x => x.MaterialID == material).ToList();
                    if (currentStorage.Count == 0)
                    {
                        var newStorage = currentAdmin;
                        newStorage[0].Quantity = 0;
                        newStorage[0].StoreId = storeId;
                        currentStorage = newStorage.ToList();
                        //Trừ của admin
                        currentAdmin[0].Quantity = currentAdmin[0].Quantity - quantity;
                        //Cộng vào kho của store
                        currentStorage[0].Quantity = quantity;
                        db.Entry(currentAdmin[0]).Property(x => x.Quantity).IsModified = true;
                        db.Storages.Add(currentStorage[0]);
                        db.SaveChanges();
                    }
                    //Trừ của admin
                    currentAdmin[0].Quantity = currentAdmin[0].Quantity - quantity;
                    //Cộng vào kho của store
                    currentStorage[0].Quantity = quantity + currentStorage[0].Quantity;
                    db.Entry(currentStorage[0]).Property(x => x.Quantity).IsModified = true;
                    db.Entry(currentAdmin[0]).Property(x => x.Quantity).IsModified = true;
                }
            }
        

            var item = db.OrderMatts.Find(id);
            if (item != null)
            {
                db.OrderMatts.Attach(item);
                item.Status = trangThai;
                db.Entry(item).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
                return Json(new { success = true, message = Message.SuccessSaveChange.ToString() });

            }
            return Json(new { success = false, message = Message.FailureSaveChange.ToString() });
        }
    }
}