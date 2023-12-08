﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Controllers
{
    public class MenuController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuTop()
        {
            var items = db.Categories.OrderBy(x => x.Position).Where(x=>x.IsActive).ToList();
            return PartialView("_MenuTop", items);
        }

        public ActionResult MenuProductCategory() 
        {
            var items = db.ProductCategories.ToList();
            return PartialView("_MenuProductCategory", items);
        }

        public ActionResult MenuLeft(int? id)
        {
            // Lấy ra danh sách cửa hàng
            var store = db.Stores.ToList();
            ViewBag.StoreList = store;

            if(id != null)
            {
                ViewBag.CateId = id;
            }    
            var items = db.ProductCategories.ToList();
            return PartialView("_MenuLeft", items);
        }
        public ActionResult GetStoresByProvince(string province)
        {
            var storesInProvince = db.Stores.Where(s => s.Province == province).ToList();
            return PartialView("_StoresInProvince", storesInProvince);
        }
        public ActionResult MenuArrivals()
        {
            var items = db.ProductCategories.ToList();
            return PartialView("_MenuArrivals", items);
        }
    }
}