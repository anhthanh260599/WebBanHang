using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,StoreEmployee,Store,StoreManager,Coordinator,Warehouse")]
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Home
        public ActionResult Index()
        {
            DateTime today = DateTime.Today;
            ViewBag.NewOrderToDay = db.Orders.Where(x => DbFunctions.TruncateTime(x.CreateDate) == today && x.Status == 3).Count();

            var newOrderToday = 0;
            decimal totalAmountToday = 0;
            decimal ticketAccount = 0;

            if (ViewBag.NewOrderToDay > 0)
            {
                newOrderToday = ViewBag.NewOrderToDay;
                totalAmountToday = db.Orders.Where(x => DbFunctions.TruncateTime(x.CreateDate) == today).Sum(x => x.TotalAmount);
            }
            else
            {
                newOrderToday = 1;
            }
            ticketAccount = totalAmountToday / newOrderToday;
            ViewBag.TicketAccount = ticketAccount;

            // Khởi tạo UserManager và RoleManager
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            // Lấy store id
            var userId = User.Identity.GetUserId();
            var user = userManager.FindById(userId);
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            var storeID = db.Stores.Where(x => x.UserID == currentUser.Id).Select(x => x.Id).FirstOrDefault();
            ViewBag.storeID = storeID;

            // Tìm role "Customer" bằng tên
            var customerRole = roleManager.FindByName("Customer");

            // Kiểm tra nếu role tồn tại
            if (customerRole != null)
            {
                // Đếm số lượng người dùng có vai trò "Customer"
                int customerCount = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM AspNetUsers INNER JOIN AspNetUserRoles ON AspNetUsers.Id = AspNetUserRoles.UserId INNER JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id WHERE AspNetRoles.Name = 'Customer'").Single();

                // Gán giá trị cho ViewBag
                ViewBag.UserAccount = customerCount;
            }

            var item = new ThongKeViewModelString();
            item.HomNay = HttpContext.Application["HomNay"].ToString();
            ViewBag.TruyCapHomNay = item.HomNay;

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

        public ActionResult Partial_TopStore()
        {
            var query = (from s in db.Stores
                        join od in db.Orders on s.Id equals od.StoreID
                        join odd in db.OrderDetails on od.Id equals odd.OrderID
                        group new { s, odd } by s.Name into g
                        orderby g.Sum(x => x.odd.Price * x.odd.Quantity) descending
                        select new
                        {
                            StoreName = g.Key,
                            TotalRevenue = g.Sum(x => x.odd.Price * x.odd.Quantity)
                        }).ToList();

            ViewBag.TopStores = Newtonsoft.Json.JsonConvert.SerializeObject(query); // chuyen sang json


            return PartialView();

        }

        public ActionResult Partial_RevenueByStore(int idStore)
        {
            var query = from od in db.Orders
                        join odt in db.OrderDetails on od.Id equals odt.OrderID
                        where od.StoreID == idStore
                        group new { od, odt } by EntityFunctions.TruncateTime(od.CreateDate) into g
                        orderby g.Key
                        select new
                        {
                            OrderDate = SqlFunctions.DatePart("day", g.Key) + "-" +
                                         SqlFunctions.DatePart("month", g.Key) + "-" +
                                         SqlFunctions.DatePart("year", g.Key),
                            TotalRevenue = g.Sum(x => x.odt.Price * x.odt.Quantity)
                        };

            var result = query.Take(5).ToList();
            ViewBag.TopStores = Newtonsoft.Json.JsonConvert.SerializeObject(result); // chuyen sang json

            return PartialView();
        }

    }
}