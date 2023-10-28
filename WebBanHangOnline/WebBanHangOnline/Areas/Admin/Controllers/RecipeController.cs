using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class RecipeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var items = db.Recipes.OrderByDescending(x => x.DateEdit).ToList();
            return View(items);
        }

        public ActionResult Add()
        {
            ViewBag.ListMaterials = db.Matterials.ToList();
            ViewBag.ListProducts = new SelectList(db.Products.ToList(), "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Recipe recipe, List<int> selectedMaterials)
        {
            try
            {
                ViewBag.ListMaterials = db.Matterials.ToList();
                ViewBag.ListProducts = new SelectList(db.Products.ToList(), "Id", "Title");


                return RedirectToAction("Index");
            }
            catch
            {
                return View(recipe);

            }

        }
    }
}