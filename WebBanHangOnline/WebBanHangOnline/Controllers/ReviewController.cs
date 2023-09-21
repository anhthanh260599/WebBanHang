﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    public class ReviewController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Review
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Partial_AddReview(int productId)
        {
            ViewBag.ProductId = productId;
            var item = new ReviewProduct();
            if(User.Identity.IsAuthenticated)
            {
                var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = userManager.FindByName(User.Identity.Name);
                if(user != null)
                {
                    item.Email = user.Email;
                    item.FullName = user.FullName;
                    item.UserName = user.UserName;
                    item.Avatar = user.Avatar;
                }
                return PartialView(item);
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult PostReview(ReviewProduct request) 
        {
            if(ModelState.IsValid)
            {
                request.CreatedDate = DateTime.Now;
                db.ReviewProducts.Add(request);
                db.SaveChanges();
                return Json(new {success = true});
            }
            return Json(new {success = false});
        }

        public ActionResult Partial_ListReview(int productId)
        {
            ViewBag.ProductId = productId;
            var items = db.ReviewProducts.Where(x=>x.ProductId == productId).OrderByDescending(x=>x.Id).ToList();
            ViewBag.Count = items.Count;
            return PartialView(items);
        }
    }
}