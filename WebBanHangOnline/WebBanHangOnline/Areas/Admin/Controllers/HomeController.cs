using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee,Store")]
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Partial_TopViewCount()
        {
            var items = db.Products.OrderByDescending(x => x.ViewCount).Take(5).ToList();
            return PartialView(items);
        }

        public ActionResult Partial_TopOrder()
        {
            List<ProductQuantityViewModel> products = new List<ProductQuantityViewModel>();

            var query = (from od in db.OrderDetails
                         join p in db.Products on od.ProductID equals p.Id
                         group new { od, p } by new { p.Id, p.ProductCode, p.Title } into g
                         select new ProductQuantityViewModel
                         {
                             ProductID = g.Key.Id,
                             ProductCode = g.Key.ProductCode,
                             ProductName = g.Key.Title,
                             TotalQuantitySold = g.Sum(x => x.od.Quantity)
                         })
                   .OrderByDescending(p => p.TotalQuantitySold)
                   .Take(5).ToList();

            return PartialView(query);
        }


        public ActionResult Partial_TopWishList()
        {
            List<WishListCountViewModel> products = new List<WishListCountViewModel>();
            var query = (from wish in db.WishLists
                         join p in db.Products on wish.ProductId equals p.Id
                         group wish by new { p.Id, p.ProductCode, p.Title } into grouped
                         select new WishListCountViewModel
                         {
                             ProductID = grouped.Key.Id,
                             ProductCode = grouped.Key.ProductCode,
                             ProductName = grouped.Key.Title,
                             WishCount = grouped.Count()
                         })
              .OrderByDescending(p => p.WishCount)
              .Take(5)
              .ToList();
            return PartialView(query);
        }
    }
}