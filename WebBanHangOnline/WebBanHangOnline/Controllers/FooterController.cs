using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Controllers
{
    public class FooterController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Footer
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Partial_FooterSocialMedia()
        {
            var items = db.SocialMediaProfiles.ToList();
            return PartialView(items);
        }
        public ActionResult Partial_FooterSocialMediaPlugin()
        {
            var items = db.SocialMediaPlugins.ToList();
            return PartialView(items);
        }

        public ActionResult Partial_FooterContactInfo()
        {
            var items = db.ContactInfos.ToList();
            return PartialView(items);
        }

    }
}