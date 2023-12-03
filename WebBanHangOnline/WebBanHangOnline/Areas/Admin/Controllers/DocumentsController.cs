using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
using Microsoft.VisualBasic.Logging;
using System.Xml.Linq;
using PayPal.Api;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class DocumentsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Documents
        public ActionResult Index()
        {
            var items = db.Documents.OrderByDescending(d => d.Id).ToList();
            return View(items);
        }

        public FileResult Download(int id)
        {
            // Lấy thông tin tài liệu dựa trên id
            var document = db.Documents.Find(id);
            // Giải mã đường dẫn tệp trước khi sử dụng nó
            var decodedFilePath = HttpUtility.UrlDecode(document.File);
            // Xây dựng đường dẫn đến tệp tải xuống trên máy chủ
            var filePath = Server.MapPath(decodedFilePath); // Thay đổi đường dẫn thư mục tùy theo vị trí của tệp

            // Trả về tệp để tải xuống
            return File(filePath, "application/octet-stream", document.File);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Documents model)
        {
            if (ModelState.IsValid)
            {
                model.SetCreated();
                db.Documents.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var item = db.Documents.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Documents model)
        {
            if (ModelState.IsValid)
            {
                model.SetModified();
                db.Documents.Attach(model);
                model.CreateDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var item = db.Documents.Find(id);
            if (item != null)
            {
                db.Documents.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteSelected(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.Documents.Find(Convert.ToInt32(item));
                        db.Documents.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult Duplicate(int id)
        {

            var item = db.Documents.Find(id);
            if (item != null)
            {
                // Sử dụng Prototype
                var cloneDocument = item.Clone();
                db.Documents.Add((Documents)cloneDocument);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}