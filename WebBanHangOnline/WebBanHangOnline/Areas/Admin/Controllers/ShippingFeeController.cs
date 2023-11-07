using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.Common;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class ShippingFeeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/ShippingFee
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UpdateShippingFee()
        {
            // Kiểm tra xem có đối tượng ShippingFee nào có giá trị không
            var shippingFee = db.ShippingFees.FirstOrDefault();

            if (shippingFee != null)
            {
                // Đã có giá trị, điều hướng tới trang Edit
                return RedirectToAction("Edit", new { id = shippingFee.Id });
            }
            else
            {
                // Chưa có giá trị, điều hướng tới trang Add
                return RedirectToAction("Add");
            }
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ShippingFee model)
        {
            if (ModelState.IsValid)
            {
                if(model.Value >= 0)
                {
                    db.ShippingFees.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");   
                }
                else
                {
                    ViewBag.KhongDuocNhapSoAm = "Không được nhập số âm";
                }
            }
            return View();
        }

        public ActionResult Edit(int id)
        {
            var shippingFee = db.ShippingFees.FirstOrDefault(sf => sf.Id == id);

            if (shippingFee != null)
            {
                return View(shippingFee);
            }
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShippingFee model)
        {
            if (ModelState.IsValid)
            {
                if (model.Value >= 0)
                {
                    db.ShippingFees.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ViewBag.KhongDuocNhapSoAm = "Không được nhập số âm";
                }
            }
            return View(model);
        }
    }
}