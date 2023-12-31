using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using static WebBanHangOnline.Areas.Admin.Controllers.StoreController;

namespace WebBanHangOnline.Controllers
{
    public class StoreController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Store
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData()
        {
            var items = db.Stores.Where(x=>x.IsActive)
                .Select(x=> new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Alias = x.Alias,
                    Image = x.Image,
                    LinkMap = x.LinkMap,
                    EmbedMap = x.EmbedMap,
                    Address = x.Address,
                    Province = x.Province,
                    District = x.District,
                    Ward = x.Ward,
                    StoreManagerName = x.StoreManagerName,
                    StorePhone = x.StorePhone,
                })
                .ToList();
            return Json(items,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int id)
        {
            var item = db.Stores.Find(id);
            return View(item);
        }
    }
}