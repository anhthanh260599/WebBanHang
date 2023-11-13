﻿using Microsoft.AspNet.Identity.EntityFramework;
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
                orderMatts.Address = "FourC Sư Vạn Hạnh";
            }

            orderMatts.TotalAmount = total;
            orderMatts.Status = 1;

            orderMatts.CreateDate = DateTime.Now.AddHours(14);
            orderMatts.ModifierDate = DateTime.Now.AddHours(14);

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
            var items = db.OrderMatts.OrderByDescending(x=>x.Id).ToList();
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
        public ActionResult UpdateTrangThai(int id, int trangThai)
        {
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