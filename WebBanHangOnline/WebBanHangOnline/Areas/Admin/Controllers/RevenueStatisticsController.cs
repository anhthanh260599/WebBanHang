using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class RevenueStatisticsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // Thống kê doanh thu
        // GET: Admin/RevenueStatistics
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetStatistical (string fromDate, string toDate)
        {


            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            var storeID = db.Orders.Where(x => x.Store.UserID == currentUser.Id).Select(x => x.Store.Id).ToList();

            bool isAdmin = User.IsInRole("Admin"); // Kiểm tra xem người dùng có vai trò "Admin" hay không


            var queryTotalSell = from o in db.Orders
                                 where o.Status == 3 && (isAdmin || (o.StoreID.HasValue && storeID.Contains(o.StoreID.Value)))
                                 select new
                                 {
                                     CreateDate = o.CreateDate,
                                     Code = o.Code,
                                     TotalAmount = o.TotalAmount,
                                     DiscountAmount = o.DiscountAmount,
                                     StoreID = o.StoreID
                                 };
            var queryTotalBuy = from od in db.OrderDetails
                                join p in db.Products on od.ProductID equals p.Id
                                join o in db.Orders on od.OrderID equals o.Id
                                where o.Status == 3 && (isAdmin || (o.StoreID.HasValue && storeID.Contains(o.StoreID.Value)))
                                select new
                                {
                                    CreateDate = o.CreateDate,
                                    ProductID = od.ProductID,
                                    Quantity = od.Quantity,
                                    OriginalPrice = p.OriginalPrice,
                                    StoreID = o.StoreID
                                };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate,"yyyy-MM-dd",null);
                queryTotalSell = queryTotalSell.Where(x=>x.CreateDate >= startDate);
                queryTotalBuy = queryTotalBuy.Where(x => x.CreateDate >= startDate);
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "yyyy-MM-dd", null);
                queryTotalSell = queryTotalSell.Where(x => x.CreateDate <= endDate);
                queryTotalBuy = queryTotalBuy.Where(x => x.CreateDate <= endDate);
            }
            var result = queryTotalSell.GroupBy(x => DbFunctions.TruncateTime(x.CreateDate))
                              .Select(x => new
                              {
                                  Date = x.Key.Value,
                                  TotalBuy = queryTotalBuy
                                             .Where(y => DbFunctions.TruncateTime(y.CreateDate) == x.Key.Value)
                                             .Sum(y => y.Quantity * y.OriginalPrice),
                                  TotalSell = x.Sum(y => y.TotalAmount),
                              })
                              .Select(x => new
                              {
                                  Date = x.Date,
                                  DoanhThu = x.TotalSell,
                                  LoiNhuan = x.TotalSell - x.TotalBuy
                              });
            return Json(new {Data = result}, JsonRequestBehavior.AllowGet);
        }
    }
}