﻿using PayPal.Api;
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

        public ActionResult List_API()
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

        public ActionResult CustomerRequest()
        {
            ViewBag.RequestType = new SelectList(db.RequestTypes.ToList(), "Id", "RequestTypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerRequest(CustomerRequest model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.RequestType = new SelectList(db.RequestTypes.ToList(), "Id", "RequestTypeName");
                if (model.RequestType != null) 
                {
                    db.CustomerRequests.Add(new CustomerRequest
                    {
                        CustomerName = model.CustomerName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        CreatedDate = DateTime.Now,
                        RequestTitle = model.RequestTitle,
                        RequestContent = model.RequestContent,
                        RequestTypeId = model.RequestType.Id,
                        IsResolve = false
                    });
                    db.SaveChanges();
                }
                return RedirectToAction("CustomerRequest_SuccessSend", "Home");
            }
            return View("CustomerRequest");
        }

        public ActionResult CustomerRequest_SuccessSend()
        {
            return View();
        }

        public ActionResult TraCuuDonHang(string request)
        {
            List<Models.EF.Order> danhSachDonHang = new List<Models.EF.Order>();

            if (!string.IsNullOrEmpty(request))
            {
                // Thực hiện tìm kiếm trong cơ sở dữ liệu sử dụng Entity Framework
                danhSachDonHang = db.Orders
                   .Where(d => d.Code.Equals(request, StringComparison.OrdinalIgnoreCase) || d.Phone.Equals(request, StringComparison.OrdinalIgnoreCase))
                   .ToList();
            }
            return View(danhSachDonHang);
        }

        public ActionResult TraCuuDonHangDetail(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_TraCuuDonHangDetail(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderID == id).ToList();
            return PartialView(items);
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