using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.Common;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Partial_HomeSlider()
        {
            var items = db.Sliders.ToList();
            return PartialView(items);
        }

        public ActionResult Partial_NewItem()
        {
            var item = db.Posts.Where(x=>x.Alias == "san-pham-moi" && x.IsActive).FirstOrDefault();
            return PartialView(item);
        }

        public ActionResult Partial_Subscribe()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subscribe(Subscribe request)
        {
            var emailBrand = db.ContactInfos.Where(x => x.InfoName == "Email").Select(x=>x.InfoValue).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var checkItem = db.Subscribes.FirstOrDefault(x => x.Email == request.Email);
                if (checkItem != null)
                {
                    return Json(new { success = false });
                }

                db.Subscribes.Add(new Subscribe
                {
                    Email = request.Email,
                    CreateDate = DateTime.Now,
                });
                db.SaveChanges();

                //Send Mail

                string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/sendMailSubscribeKH.html"));
                contentCustomer = contentCustomer.Replace("{{EmailDangKy}}", request.Email);
                contentCustomer = contentCustomer.Replace("{{Brand}}", Message.Brand.ToString());
                contentCustomer = contentCustomer.Replace("{{EmailBrand}}", emailBrand.ToString());
                Common.SendMail(Message.Brand.ToString(), "Thông tin đăng ký tài khoản tại " + Message.Brand.ToString(), contentCustomer.ToString(), request.Email);

                //End Send Mail

                return Json(new {success = true});
            }
            return View("Partial_Subscribe",request);
        }

        // Hàm dùng để thống kê số lượt truy cập
        public ActionResult Refresh()
        {
            var item = new ThongKeViewModelString();

            ViewBag.Visitors_online = HttpContext.Application["visitors_online"];

            item.HomNay = HttpContext.Application["HomNay"].ToString();
            item.HomQua =  HttpContext.Application["HomQua"].ToString();
            item.TuanNay = HttpContext.Application["TuanNay"].ToString();
            item.TuanTruoc = HttpContext.Application["TuanTruoc"].ToString();
            item.ThangNay = HttpContext.Application["ThangNay"].ToString();
            item.ThangTruoc = HttpContext.Application["ThangTruoc"].ToString();
            item.TatCa = HttpContext.Application["TatCa"].ToString();
            return PartialView(item);
        }
    }
}