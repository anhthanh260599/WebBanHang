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

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class StoreOrderController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/StoreOrder

        public ActionResult Index()
        {
            var items = db.Matterials.Where(x => x.IsActive).ToList();
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
            orderMatts.StoreID = 14;
            orderMatts.Code = "Test";
            orderMatts.CustomerName = "Test";
            orderMatts.Phone = "0919568158";
            orderMatts.Address = "test";
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
            var items = db.OrderMatts.ToList();
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
    }
}