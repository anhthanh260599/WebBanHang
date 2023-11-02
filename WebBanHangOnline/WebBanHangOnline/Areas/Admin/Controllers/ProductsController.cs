using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Products
        public ActionResult Index()
        {
            var items = db.Products.OrderByDescending(x => x.Id).ToList();
            return View(items);
        }

      
        public ActionResult Add() 
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product model, List<string> Images,List<int> rDefault)  
        {
            try
            {
                ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
                if (Images != null && Images.Count > 0)
                {
                    for (int i = 0; i < Images.Count; i++)
                    {
                        if (i + 1 == rDefault[0])
                        {
                            model.Image = Images[i];
                            model.ProductImage.Add(new ProductImage
                            {
                                ProductID = model.Id,
                                Image = Images[i],
                                IsDefault = true
                            });
                        }
                        else
                        {
                            model.ProductImage.Add(new ProductImage
                            {
                                ProductID = model.Id,
                                Image = Images[i],
                                IsDefault = false
                            }); ;
                        }

                    }
                }
                model.CreateDate = DateTime.Now;
                model.ModifierDate = DateTime.Now;
                if (string.IsNullOrEmpty(model.SeoTitle))
                {
                    model.SeoTitle = model.Title;
                }
                if (string.IsNullOrEmpty(model.Alias))
                {
                    model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);
                }
                db.Products.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
                //return Json(new { id = model.Id });
            }
            catch
            {
                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            var item = db.Products.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                model.ModifierDate = DateTime.Now;
                model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);
                db.Products.Attach(model);
                db.Entry(model).State=System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Products.Find(id);
            if(item != null)
            {
                db.Products.Remove(item);
                db.SaveChanges();
                return Json(new {success = true});
            }
            return Json(new {success = false});
        }

        [HttpPost]
        public ActionResult IsActive(int id) 
        {
            var item = db.Products.Find(id);
            if(item != null)
            {
                item.IsActive =! item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new {success = true , IsActive = item.IsActive});
            }
            return Json(new {success = false});
        }

        [HttpPost]
        public ActionResult IsHome(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsHome = !item.IsHome;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsHome = item.IsHome });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsHot(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsHot = !item.IsHot;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsHot = item.IsHot });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsFeature(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.IsFeature = !item.IsFeature;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsFeature = item.IsFeature });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteSelected(string ids)
        {
            if(!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if(items!=null && items.Any())
                {
                    foreach(var item in items)
                    {
                        var obj = db.Products.Find(Convert.ToInt32(item));
                        db.Products.Remove(obj);
                        db.SaveChanges();
                    }
                }   
                return Json(new {success = true});
            }    
            return Json(new {success =false});
        }
    }
}