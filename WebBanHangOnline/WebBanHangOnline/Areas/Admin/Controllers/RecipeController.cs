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
            //var productsInRecipes = db.Recipes.Select(x => x.ProductID).ToList();
            //var productsNotInRecipes = db.Products.ToList().Where(product => !productsInRecipes.Contains(product.Id)).ToList();

            ViewBag.ListMaterials = db.Matterials.ToList();
            ViewBag.ListProducts = new SelectList(db.Products.ToList(), "Id", "Title");

            return View();
        }

        [HttpPost]
        public ActionResult Add(Recipe recipe, List<RecipeDetail> listRecipeDetail)
        {
            try
            {
                //var productsInRecipes = db.Recipes.Select(x => x.ProductID).ToList();
                //var productsNotInRecipes = db.Products.ToList().Where(product => !productsInRecipes.Contains(product.Id)).ToList();

                //var missingMaterials = db.Matterials.ToList().Where(material => !items.Any(item => item.MatterialID == material.Id)).ToList();

                ViewBag.ListMaterials = db.Matterials.ToList();
                ViewBag.ListProducts = new SelectList(db.Products.ToList(), "Id", "Title");

                using (var context = new ApplicationDbContext())
                {
                    recipe.DateCreate = DateTime.Now;
                    recipe.DateEdit = DateTime.Now;
                    var itemProduct = db.Products.Where(s => s.Id == recipe.ProductID).FirstOrDefault();
                    recipe.Code = "CT-" + itemProduct.ProductCode;
                    context.Recipes.Add(recipe);

                    for (int i = 0; i < listRecipeDetail.Count; i++)
                    {

                        listRecipeDetail[i].RecipeID = recipe.ID;
                        listRecipeDetail[i].RecipeProductID = recipe.ProductID;
                        context.RecipeDetails.Add(listRecipeDetail[i]);
                    }
                    context.SaveChanges();
                }
                return Json(new { newUrl = Url.Action("Index", "Recipe") });
            }
            catch
            {
                return View(recipe);
            }

        }

        public ActionResult Edit(int RecipeID, int RecipeProductID)
        {
            /*ViewBag.ListMaterials = db.Matterials.ToList();*/
            var items = db.RecipeDetails.Where(s => s.RecipeID == RecipeID && s.RecipeProductID == RecipeProductID).ToList();
            // Lọc ra danh sách Materials không tồn tại trong items
            var missingMaterials = db.Matterials.ToList().Where(material => !items.Any(item => item.MatterialID == material.Id)).ToList();

            // Đưa danh sách missingMaterials vào ViewBag
            ViewBag.ListMaterials = missingMaterials;

            return View(items);
        }

        [HttpPost]
        public ActionResult Edit(List<RecipeDetail> listEditRecipeDetail, List<RecipeDetail> listAddRecipeDetail, Recipe recipe)
        {
            try
            {
                var itemRecipe = db.Recipes.Where(s => s.ProductID == recipe.ProductID && s.ID == recipe.ID).FirstOrDefault();
                itemRecipe.DateEdit = DateTime.Now;
                db.Entry(itemRecipe).State = System.Data.Entity.EntityState.Modified;

                //Edit
                for (int i = 0; i < listEditRecipeDetail.Count; i++)
                {
                    RecipeDetail recipeDetailEdit = listEditRecipeDetail[i];
                    db.RecipeDetails.Attach(recipeDetailEdit);
                    db.Entry(recipeDetailEdit).State = System.Data.Entity.EntityState.Modified;
                    //Add
                    if (listAddRecipeDetail != null)
                    {
                        for (int j = 0; i < listAddRecipeDetail.Count; i++)
                        {
                            RecipeDetail recipeDetailAdd = listAddRecipeDetail[i];
                            recipeDetailAdd.RecipeID = recipeDetailEdit.RecipeID;
                            recipeDetailAdd.RecipeProductID = recipeDetailEdit.RecipeProductID;

                            db.RecipeDetails.Add(recipeDetailAdd);
                        }
                    }
                }
               
                db.SaveChanges();
                return Json(new { newUrl = Url.Action("Index", "Recipe") });
            }
            catch
            {
                return View();
            } 
        }

        [HttpPost]
        public ActionResult Delete(int recipeID, int recipeProductID, int matterialID)
        {
            try
            {
                var item = db.RecipeDetails.Find(recipeID, recipeProductID, matterialID);
                if (item != null)
                {
                    db.RecipeDetails.Remove(item);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }catch
            {
                return View();
            }
        }
    }
}