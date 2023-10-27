using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
using static WebBanHangOnline.Areas.Admin.Controllers.StoreController;


namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class StoreController : Controller
    {
        public class Province
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class District
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class Ward
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Store
        public ActionResult Index()
        {
            var items = db.Stores.OrderByDescending(x=>x.Id).ToList();
            return View(items);
        }

        public async Task<ActionResult> Add()
        {
            var store = new Store();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var allUsers = userManager.Users.ToList();

            var usersInRole = allUsers.Where(u => userManager.IsInRole(u.Id, "Store")).ToList();
            var userSelectList = new SelectList(usersInRole, "Id", "UserName");
            ViewBag.UserList = userSelectList;

            var userStoreManager = allUsers.Where(u => userManager.IsInRole(u.Id, "StoreManager")).ToList();
            var userStoreManagerSelectList = new SelectList(userStoreManager, "FullName", "FullName");
            ViewBag.UserStoreManagerList = userStoreManagerSelectList;

            await PopulateDropdownLists(store);
            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Store store)
        {
            if (ModelState.IsValid)
            {
                store.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(store.Name);  // chuyển có dấu thành không dấu, mục đích để làm url sau này
                var provinceCode = store.Province;
                var districtCode = store.District;
                var wardCode = store.Ward;
                // Sử dụng mã Code để tìm tên tương ứng từ danh sách tỉnh/thành, quận/huyện và phường/xã

                // Lưu tên vào các thuộc tính của store
                store.Province = GetProvinceName(provinceCode);
                store.District = GetDistrictName(districtCode);
                store.Ward = GetWardName(wardCode);


                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var allUsers = userManager.Users.ToList();

                var usersInRole = allUsers.Where(u => userManager.IsInRole(u.Id, "Store")).ToList();
                var userSelectList = new SelectList(usersInRole, "Id", "UserName");
                ViewBag.UserList = userSelectList;

                var userStoreManager = allUsers.Where(u => userManager.IsInRole(u.Id, "StoreManager")).ToList();
                var userStoreManagerSelectList = new SelectList(userStoreManager, "FullName", "FullName");
                ViewBag.UserStoreManagerList = userStoreManagerSelectList;

                var userAccount = store.UserID;
                var AccountLogin = db.Users.Where(x=>x.Id == userAccount).Select(x=>x.UserName).FirstOrDefault();
                store.AccountLogin = AccountLogin;

                if (store.StoreManagerName == null)
                {
                    store.StoreManagerName = "Chưa có quản lý";
                }

                if (store.AccountLogin == null)
                {
                    store.AccountLogin = "Chưa có tài khoản đăng nhập";
                }

                db.Stores.Add(store);
                db.SaveChanges();
                return RedirectToAction("Index"); // Chuyển hướng sau khi tạo sản phẩm thành công
            }
            // Nếu có lỗi xảy ra, điền lại Dropdownlist và hiển thị lại trang Create
            PopulateDropdownLists(store);
            return View(store);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var item = db.Stores.Find(id);
            var store = new Store();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var allUsers = userManager.Users.ToList();

            var usersInRole = allUsers.Where(u => userManager.IsInRole(u.Id, "Store")).ToList();
            var userSelectList = new SelectList(usersInRole, "Id", "UserName");
            ViewBag.UserList = userSelectList;

            var userStoreManager = allUsers.Where(u => userManager.IsInRole(u.Id, "StoreManager")).ToList();
            var userStoreManagerSelectList = new SelectList(userStoreManager, "FullName", "FullName");
            ViewBag.UserStoreManagerList = userStoreManagerSelectList;

            await PopulateDropdownLists(store);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Store store)
        {
            if (ModelState.IsValid)
            {
                db.Stores.Attach(store);
                store.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(store.Name);  // chuyển có dấu thành không dấu, mục đích để làm url sau này

                var provinceCode = store.Province;
                var districtCode = store.District;
                var wardCode = store.Ward;
                // Sử dụng mã Code để tìm tên tương ứng từ danh sách tỉnh/thành, quận/huyện và phường/xã

                // Lưu tên vào các thuộc tính của store
                store.Province = GetProvinceName(provinceCode);
                store.District = GetDistrictName(districtCode);
                store.Ward = GetWardName(wardCode);

                // Nếu có lỗi xảy ra, điền lại Dropdownlist và hiển thị lại trang Create
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var allUsers = userManager.Users.ToList();

                var usersInRole = allUsers.Where(u => userManager.IsInRole(u.Id, "Store")).ToList();
                var userSelectList = new SelectList(usersInRole, "Id", "UserName");
                ViewBag.UserList = userSelectList;

                var userStoreManager = allUsers.Where(u => userManager.IsInRole(u.Id, "StoreManager")).ToList();
                var userStoreManagerSelectList = new SelectList(userStoreManager, "FullName", "FullName");
                ViewBag.UserStoreManagerList = userStoreManagerSelectList;

                var userAccount = store.UserID;
                var AccountLogin = db.Users.Where(x => x.Id == userAccount).Select(x => x.UserName).FirstOrDefault();
                store.AccountLogin = AccountLogin;

                if (store.StoreManagerName == null)
                {
                    store.StoreManagerName = "Chưa có quản lý";
                }

                if (store.AccountLogin == null)
                {
                    store.AccountLogin = "Chưa có tài khoản đăng nhập";
                }

                db.Entry(store).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index"); // Chuyển hướng sau khi tạo sản phẩm thành công
            }


            PopulateDropdownLists(store);
            return View(store);
        }

        private async Task PopulateDropdownLists(Store store)
        {
            using (var httpClient = new HttpClient())
            {
                // Lấy danh sách tỉnh/thành từ API
                var provinceResponse = await httpClient.GetStringAsync("https://provinces.open-api.vn/api/p/");
                var provinces = JsonConvert.DeserializeObject<List<Province>>(provinceResponse);
                ViewBag.ProvinceList = new SelectList(provinces, "Code", "Name");

                // Điền lại Dropdownlist Quận/Huyện và Phường/Xã nếu có dữ liệu trong model sản phẩm
                if (!string.IsNullOrEmpty(store.Province))
                {
                    var districtResponse = await httpClient.GetStringAsync($"https://provinces.open-api.vn/api/p/{store.Province}?depth=2");
                    var districts = JsonConvert.DeserializeObject<List<District>>(districtResponse);
                    ViewBag.DistrictList = new SelectList(districts, "Code", "Name");
                }

                if (!string.IsNullOrEmpty(store.District))
                {
                    var wardResponse = await httpClient.GetStringAsync($"https://provinces.open-api.vn/api/d/{store.District}?depth=2");
                    var wards = JsonConvert.DeserializeObject<List<Ward>>(wardResponse);
                    ViewBag.WardList = new SelectList(wards, "Code", "Name");
                }
                else
                {
                    // Nếu không có dữ liệu cho Quận/Huyện, tạo danh sách trống để tránh lỗi
                    //ViewBag.DistrictList = new SelectList(new List<District>(), "Code", "Name");
                    //ViewBag.WardList = new SelectList(new List<Ward>(), "Code", "Name");
                    ViewBag.DistrictList = new SelectList(new List<SelectListItem>(), "Value", "Text");
                    ViewBag.WardList = new SelectList(new List<SelectListItem>(), "Value", "Text");
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetDistricts(string provinceCode)
        {
            try
            {
                // Kiểm tra xem provinceCode có giá trị hợp lệ không (có thể sử dụng các kiểm tra khác nhau dựa trên cách bạn lưu trữ dữ liệu)

                // Sử dụng HttpClient để gửi yêu cầu lấy danh sách Quận/Huyện từ API
                using (var httpClient = new HttpClient())
                {
                    var districtResponse = await httpClient.GetStringAsync($"https://provinces.open-api.vn/api/p/{provinceCode}?depth=2");

                    // Chuyển đổi dữ liệu JSON thành danh sách các đối tượng District
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(districtResponse);
                    if (data.ContainsKey("districts"))
                    {
                        var districtArray = (JArray)data["districts"];
                        var districts = districtArray.Select(d => new District
                        {
                            Code = d["code"].ToString(),
                            Name = d["name"].ToString()
                        }).ToList();

                        // Trả về danh sách các đối tượng District dưới dạng JSON
                        return Json(districts, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { error = "Không tìm thấy danh sách Quận/Huyện." });
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                // Log lỗi hoặc trả về thông báo lỗi
                return Json(new { error = "Đã xảy ra lỗi khi tải danh sách Quận/Huyện." });
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetWards(string districtCode)
        {
            try
            {
                // Sử dụng HttpClient để gửi yêu cầu lấy danh sách Phường/Xã từ API dựa trên mã Quận/Huyện
                using (var httpClient = new HttpClient())
                {
                    var wardResponse = await httpClient.GetStringAsync($"https://provinces.open-api.vn/api/d/{districtCode}?depth=2");

                    // Chuyển đổi dữ liệu JSON thành danh sách các đối tượng District
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(wardResponse);
                    if (data.ContainsKey("wards"))
                    {
                        var wardtArray = (JArray)data["wards"];
                        var wards = wardtArray.Select(w => new District
                        {
                            Code = w["code"].ToString(),
                            Name = w["name"].ToString()
                        }).ToList();

                        // Trả về danh sách các đối tượng District dưới dạng JSON
                        return Json(wards, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { error = "Không tìm thấy danh sách Quận/Huyện." });
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                // Log lỗi hoặc trả về thông báo lỗi
                return Json(new { error = "Đã xảy ra lỗi khi tải danh sách Phường/Xã." });
            }
        }

        // Thêm các phương thức sau để lấy tên từ API
        private string GetProvinceName(string provinceCode)
        {
            using (var httpClient = new HttpClient())
            {
                var provinceResponse = httpClient.GetStringAsync($"https://provinces.open-api.vn/api/p/{provinceCode}").Result;
                var province = JsonConvert.DeserializeObject<Province>(provinceResponse);
                return province.Name;
            }
        }

        private string GetDistrictName(string districtCode)
        {
            using (var httpClient = new HttpClient())
            {
                var districtResponse = httpClient.GetStringAsync($"https://provinces.open-api.vn/api/d/{districtCode}").Result;
                var district = JsonConvert.DeserializeObject<District>(districtResponse);
                return district.Name;
            }
        }

        private string GetWardName(string wardCode)
        {
            using (var httpClient = new HttpClient())
            {
                var wardResponse = httpClient.GetStringAsync($"https://provinces.open-api.vn/api/w/{wardCode}").Result;
                var ward = JsonConvert.DeserializeObject<Ward>(wardResponse);
                return ward.Name;
            }
        }

       

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var item = db.Stores.Find(id);
            if (item != null)
            {
                db.Stores.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {

            var item = db.Stores.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsActive = item.IsActive });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteSelected(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.Stores.Find(Convert.ToInt32(item));
                        db.Stores.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

    }
}