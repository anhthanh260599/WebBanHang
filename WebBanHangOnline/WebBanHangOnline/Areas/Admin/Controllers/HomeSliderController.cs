﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.CommandPattern;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class HomeSliderController : Controller
    {
        //ApplicationDbContext db = new ApplicationDbContext();
        private readonly ApplicationDbContext db;
        private static Stack<IUndoCommand> _undoCommand = new Stack<IUndoCommand>();

        public HomeSliderController()
        {
            db = new ApplicationDbContext();
        }

        public HomeSliderController(ApplicationDbContext db, Stack<IUndoCommand> undoCommand)
        {
            this.db = db;
            _undoCommand = undoCommand;
        }

        // GET: Admin/HomeSlider
        public ActionResult Index()
        {
            var items = db.Sliders.OrderBy(x=>x.Order).ToList();
            return View(items);
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Slider model)
        {
            if (ModelState.IsValid)
            {
                db.Sliders.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var item = db.Sliders.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Slider model)
        {
            if (ModelState.IsValid)
            {
                db.Sliders.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //[HttpPost]
        //public ActionResult Delete(int id)
        //{

        //    var item = db.Sliders.Find(id);
        //    if (item != null)
        //    {
        //        db.Sliders.Remove(item);
        //        db.SaveChanges();
        //        return Json(new { success = true });
        //    }
        //    return Json(new { success = false });
        //}

        //[HttpPost]
        //public ActionResult DeleteSelected(string ids)
        //{
        //    if (!string.IsNullOrEmpty(ids))
        //    {
        //        var items = ids.Split(',');
        //        if (items != null && items.Any())
        //        {
        //            foreach (var item in items)
        //            {
        //                var obj = db.Sliders.Find(Convert.ToInt32(item));
        //                db.Sliders.Remove(obj);
        //                db.SaveChanges();
        //            }
        //        }
        //        return Json(new { success = true });
        //    }
        //    return Json(new { success = false });
        //}

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Sliders.Find(id);
            if(item != null)
            {
                var command = new DeleteCommand<Slider>(db,item.Id);
                _undoCommand.Push(command);
                command.Execute();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult Undo()
        {
            if(_undoCommand.Count > 0)
            {
                var undoCommand = _undoCommand.Pop();
                undoCommand.Undo();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteSelected(string ids) 
        {
            if(!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if(items != null && items.Any())
                {
                    var command = new DeleteMultipleCommand<Slider>(db, items);
                    _undoCommand.Push(command); 
                    command.Execute();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}