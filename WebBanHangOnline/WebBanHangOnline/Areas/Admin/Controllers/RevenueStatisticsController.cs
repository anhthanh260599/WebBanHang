using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
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
            var query = from o in db.Orders where o.Status == 3
                        join od in db.OrderDetails on o.Id equals od.OrderID
                        join p in db.Products on od.ProductID equals p.Id
                        join promo in db.Promotions on o.PromotionId equals promo.Id
                        select new
                        {
                            CreateDate = o.CreateDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice,
                            DiscountAmount = promo.DiscountAmount,
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate,"yyyy-MM-dd",null);
                query = query.Where(x=>x.CreateDate >= startDate);
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "yyyy-MM-dd", null);
                query = query.Where(x => x.CreateDate <= endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y=>y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price - y.DiscountAmount)
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy 
            });
            return Json(new {Data = result}, JsonRequestBehavior.AllowGet);
        }
    }
}