using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.Common;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        // Copy từ file AccountController.cs ở ngoài vào
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        // GET: Admin/Account
        public ActionResult Index()
        {
            var items = db.Users.OrderByDescending(x=>x.CreatedDate).ToList();
            return View(items);
        }


        public ActionResult Partial_Profile()
        {
            var user = UserManager.FindByName(User.Identity.Name);
            var item = new CreateAccountrViewModel();
            item.FullName = user.FullName;
            item.Avatar = user.Avatar;
            return PartialView(item);
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", Message.WrongUserNameOrPassword.ToString());
                    return View(model);
            }
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Create()
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAccountrViewModel model)
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { 
                    UserName = model.UserName, 
                    Email = model.Email, 
                    FullName = model.FullName, 
                    CreatedDate = DateTime.Now.AddHours(14),
                    Avatar = "/Content/template/avatar-mac-dinh.jpg",
                    Phone = model.Phone 
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if(model.Roles !=null)
                    {
                        foreach (var role in model.Roles)
                        {
                            UserManager.AddToRole(user.Id, role);
                        }
                    }
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771

                    // Send an email with this link, cái này dùng để gửi email khi tạo thành công
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {

            var item = db.Users.Find(id);
            if (item != null)
            {
                db.Users.Remove(item);
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
                var userIds = ids.Split(',');

                foreach (var userId in userIds)
                {
                    var user = db.Users.Find(userId);
                    if (user != null)
                    {
                        db.Users.Remove(user);
                    }
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public ActionResult Edit(string id)
        {
            var item = UserManager.FindById(id);
            var newUser = new EditAccountViewModel();
            if(item != null)
            {
                var rolesOfUser = UserManager.GetRoles(id);
                var roles = new List<string>();
                if (rolesOfUser != null)
                {
                    foreach (var role in rolesOfUser)
                    {
                        roles.Add(role);
                    }
                }
                newUser.UserName = item.UserName;
                newUser.Email = item.Email;
                newUser.FullName = item.FullName;
                newUser.Phone = item.Phone;
                newUser.Roles = roles;
            }
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View(newUser);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditAccountViewModel model)
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(model.Id);
                user.FullName = model.FullName;
                user.Phone = model.Phone;
                user.Email = model.Email;

                // Xóa tất cả các Roles hiện tại của người dùng
                var currentRoles = UserManager.GetRoles(user.Id);
                foreach (var role in currentRoles)
                {
                    await UserManager.RemoveFromRoleAsync(user.Id, role);
                }

                // Thêm các Roles mới được chọn
                if (model.Roles != null)
                {
                    foreach (var role in model.Roles)
                    {
                        await UserManager.AddToRoleAsync(user.Id, role);
                    }
                }

                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    //var roleOfUser = UserManager.GetRoles(user.Id);

                    //if (model.Roles != null)
                    //{
                    //    foreach (var role in model.Roles)
                    //    {
                    //        var checkRoles = roleOfUser.FirstOrDefault(x => x.Equals(model.Roles));
                    //        if(checkRoles == null)
                    //        {
                    //            UserManager.AddToRole(user.Id, role);
                    //        }
                    //    }
                    //}


                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771

                    // Send an email with this link, cái này dùng để gửi email khi tạo thành công
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}