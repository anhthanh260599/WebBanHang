using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    public class NewsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: News
        public ActionResult Index(int? page, int? pageSize)
        {
            //var pageSize = 1;
            //if (page == null)
            //{
            //    page = 1;
            //}
            //IEnumerable<News>items = db.News.OrderByDescending(x=>x.Id).Where(x=>x.IsActive).ToList();
            //var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //items = items.ToPagedList(pageIndex, pageSize);
            //ViewBag.PageSize = pageSize;
            //ViewBag.Page = page;
            //return View(items);

            // Test
            int defaultPageSize = 5; // Giá trị mặc định

            if (Session["pageSize"] != null)
            {
                defaultPageSize = (int)Session["pageSize"];
            }
            else
            {
                // Nếu không có giá trị trong Session, lưu giá trị mặc định vào Session
                Session["pageSize"] = defaultPageSize;
            }

            var pageIndex = page ?? 1;
            IEnumerable<News> items = db.News.OrderByDescending(x => x.Id).Where(x => x.IsActive).ToList();

            // Tính toán số lượng trang tối đa dựa trên số lượng items và pageSize
            int maxPageIndex = (int)Math.Ceiling((double)items.Count() / defaultPageSize);

            // Kiểm tra và cập nhật lại số trang nếu cần
            if (pageIndex > maxPageIndex)
            {
                pageIndex = maxPageIndex;
            }

            items = items.ToPagedList(pageIndex, defaultPageSize);

            ViewBag.PageSize = defaultPageSize;
            ViewBag.Page = pageIndex;

            return View(items);

        }

        [HttpPost]
        public ActionResult SavePageSize(int pageSize)
        {
            Session["pageSize"] = pageSize;
            return Json(new { success = true });
        }


        public ActionResult Detail(int id)
        {
            var item = db.News.Find(id);
            return View(item);
        }

        public ActionResult Partial_News_Home()
        {
            var items = db.News.OrderByDescending(x=>x.Id).Where(x=>x.IsActive).Take(3);
            return PartialView(items);
        }
    }
}