using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class StoreOrderController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/StoreOrder
        public ActionResult Index()
        {
            var items = db.Matterials.Where(x => x.IsActive).ToList();
            return View(items);
        }


        ///////////////// Thông làm phần này đi
    }
}