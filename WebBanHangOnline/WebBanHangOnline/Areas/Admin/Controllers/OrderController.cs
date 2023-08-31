using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.Common;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Order
        public ActionResult Index()
        {
            var items = db.Orders.OrderByDescending(x=>x.CreateDate).ToList();
            return View(items);
        }

        public ActionResult Detail(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_Detail(int id)
        {
            var items = db.OrderDetails.Where(x=>x.OrderID == id).ToList();  
            return PartialView(items);
        }

        [HttpPost]
        public ActionResult UpdateTrangThai(int id, int trangThai)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                db.Orders.Attach(item);
                item.TypePayment = trangThai;
                db.Entry(item).Property(x=>x.TypePayment).IsModified = true;
                db.SaveChanges();
                return Json(new { success = true, message = Message.SuccessSaveChange });

            }
            return Json(new {success  = false, message = Message.FailureSaveChange });
        }
    }
}