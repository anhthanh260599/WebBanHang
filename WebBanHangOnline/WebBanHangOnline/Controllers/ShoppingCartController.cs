using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.Common;
using WebBanHangOnline.Models.EF;
using System.IO;
using System.Configuration;
using WebBanHangOnline.Models.Payment;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using PayPal.Api;
using System.Security.Policy;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.EnterpriseServices.CompensatingResourceManager;
using WebBanHangOnline.Models.MySingleton;
using Newtonsoft.Json;

namespace WebBanHangOnline.Controllers
{
    public class ShoppingCartController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        decimal phiShip;

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private readonly string _clientId;
        private readonly string _secretKey;

        public ShoppingCartController()
        {
            phiShip = db.ShippingFees.Select(x => x.Value).FirstOrDefault();
            if (phiShip == null)
            {
                phiShip = 0;
            }
        }

        public ShoppingCartController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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


        // GET: ShoppingCart
        public ActionResult Index()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }

        public ActionResult VnPay_Return()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();
                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.Orders.FirstOrDefault(x => x.Code == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.Status = 2;//đã thanh toán
                            itemOrder.IsConfirm = true;

                            // Điều phối đơn xuống cửa hàng
                            int storeID = StoreSingleton.Instance.Id; // Lấy ra id của cửa hàng mà khách hàng chọn
                            itemOrder.StoreID = storeID;
                            itemOrder.Status = 6;

                            //if (itemOrder.StoreID != null)
                            //{
                            //    itemOrder.Status = 6;
                            //}

                            db.Orders.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            //// Send Mail cho Khách hàng ////
                            var item = db.OrderDetails.Where(s => s.OrderID == itemOrder.Id).ToList();

                            var strSanPham = "";
                            var thanhTien = decimal.Zero;
                            var tongTien = decimal.Zero;
                            var khuyenMai = decimal.Zero;
                            string stringSubTotal = null;
                            foreach (var sp in item)
                            {
                                strSanPham += "<tr>";
                                strSanPham += "<td>" + sp.Product.Title + "</td>";
                                strSanPham += "<td>" + sp.Quantity + "</td>";
                                strSanPham += "<td>" + Common.FormatNumber(sp.Price, 0) + "</td>";
                                strSanPham += "</tr>";

                                thanhTien += sp.Price * sp.Quantity;
                                stringSubTotal = thanhTien.ToString();
                            }

                            khuyenMai = itemOrder.DiscountAmount;

                            if (itemOrder.PromotionId != null)
                            {
                                tongTien = thanhTien - khuyenMai;
                            }

                            if (itemOrder.PromotionId == null)
                            {
                                tongTien = thanhTien;
                            }
                            tongTien = tongTien + phiShip;
                            var trangThaiDon = string.Empty;
                            var hinhThucThanhToan = string.Empty;

                            if (itemOrder.Status == 2 || itemOrder.Status == 6)
                            {
                                trangThaiDon = "<p>Trạng thái đơn hàng: <strong style=\"color:green;\">Đã thanh toán</strong></p>\r\n<p style=\"margin:0 0 16px\">\r\nChúng tôi đang tiến hành hoàn thiện đơn\r\nđặt hàng của bạn\r\n</p>";
                                hinhThucThanhToan = "VNPAY";
                            }
                            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/sendMailKhachHang.html"));
                            contentCustomer = contentCustomer.Replace("{{MaDon}}", itemOrder.Code);
                            contentCustomer = contentCustomer.Replace("{{TrangThaiDon}}", trangThaiDon);
                            contentCustomer = contentCustomer.Replace("{{HinhThucThanhToan}}", hinhThucThanhToan);
                            contentCustomer = contentCustomer.Replace("{{NgayDat}}", itemOrder.CreateDate.ToString());
                            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", itemOrder.CustomerName);
                            contentCustomer = contentCustomer.Replace("{{Phone}}", itemOrder.Phone);
                            contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", itemOrder.Address);
                            contentCustomer = contentCustomer.Replace("{{Email}}", itemOrder.Email);
                            contentCustomer = contentCustomer.Replace("{{GhiChu}}", itemOrder.Notes);
                            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                            contentCustomer = contentCustomer.Replace("{{PhiShip}}", Common.FormatNumber(phiShip, 0));
                            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", Common.FormatNumber(thanhTien, 0));
                            contentCustomer = contentCustomer.Replace("{{KhuyenMai}}", Common.FormatNumber(khuyenMai, 0));
                            contentCustomer = contentCustomer.Replace("{{TongTien}}", Common.FormatNumber(tongTien, 0));
                            Common.SendMail(Message.Brand.ToString(), "Đơn hàng #" + itemOrder.Code, contentCustomer.ToString(), itemOrder.Email);
                        }
                        //Thanh toan thanh cong
                        ViewBag.ThanhToanThanhCong = "Cảm ơn quý khách đã sử dụng dịch vụ. Giao dịch được thực hiện thành công.";
                        ViewBag.InnerText = $"{itemOrder.Code}";
                        ViewBag.EmailThanhToan = $"{itemOrder.Email}";

                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        ViewBag.ThanhToanThatBai = "Thanh toán thất bại, vui lòng kiểm tra lại giao dịch";
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    ViewBag.SoTienThanhToan = "Số tiền thanh toán (VND):" + Common.FormatNumber(vnp_Amount); // vnp_Amount.ToString();
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }
            //var a = UrlPayment(0, "DH3574");
            return View();
        }

        public ActionResult CheckoutSuccess()
        {
            string email = Session["EmailCustomer"] as string;
            return View();
        }

        public ActionResult Checkout()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }

        public ActionResult Partial_Checkout()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            if (user != null)
            {
                ViewBag.User = user;
            }

            // Ứng dụng Singleton
            int storeID = StoreSingleton.Instance.Id;
            var itemStore = db.Stores.Where(s => s.Id == storeID).FirstOrDefault();
            ViewBag.StoreByID = $"{itemStore.Name} -- {itemStore.Address}, {itemStore.District}, {itemStore.Ward}, {itemStore.Province}";

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(OrderViewModel request)
        {

            if (request.TypePayment == -1)
            {
                return Json(new { Success = false, message = "Vui lòng chọn phương thức thanh toán", errorcode = 69 });
            }

            var code = new { Success = false, Code = -1, Url = "" };
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    Models.EF.Order order = new Models.EF.Order();
                    order.CustomerName = request.CustomerName;
                    order.Address = request.Address;
                    order.Phone = request.Phone;
                    order.Status = 1; // 1 = Chưa thanh toán, 2 = Đã thanh toán, 3 = Hoàn thành giao, 4 = Đã huỷ, 5 = Đang giao hàng
                    order.Notes = request.Notes;
                    cart.Items.ForEach(x => order.OrderDetails.Add(new OrderDetail
                    {
                        ProductID = x.ProductId,
                        Price = x.Price,
                        Quantity = x.Quantity,
                    }));

                    int storeID = 0; // Giá trị mặc định khi không có StoreID
                    foreach (var item in cart.Items)
                    {
                        if (item.ProductStoreID != 0)
                        {
                            storeID = item.ProductStoreID;
                            order.StoreID = storeID;
                            break; // Đã tìm thấy một sản phẩm có StoreID khác 0, thoát khỏi vòng lặp
                        }
                    }

                    if (order.StoreID != null)
                    {
                        order.Status = 6;
                    }

                    //order.StoreID = storeID;

                    if (User.Identity.IsAuthenticated)
                    {
                        order.CustomerID = User.Identity.GetUserId();
                    }

                    if (cart.PromotionId != 0)
                    {
                        order.PromotionId = cart.PromotionId;
                        order.PromotionCode = cart.PromotionCode.ToUpper();
                        order.TypePromotion = cart.TypePromotion;
                        order.DiscountAmount = cart.DiscountAmount;
                        var promotionItem = db.Promotions.Where(x => x.Id == order.PromotionId).FirstOrDefault();

                        //khuyến mãi
                        //Lấy user hiện tại
                        int check = 0;
                        if (User.Identity.IsAuthenticated)
                        {
                            var user = db.Users.Where(x => x.Id == order.CustomerID).FirstOrDefault();
                            check = user.CheckPoint;
                            if (promotionItem.Point <= check)
                            {
                                promotionItem.Quantity = promotionItem.Quantity - 1;
                            }
                            else
                            {
                                ViewBag.NotEnoughtPoint = "Không đủ điểm để sử dụng";

                            }
                        }
                    }



                    order.Quantity = cart.Items.Sum(x => x.Quantity);
                    order.Email = request.Email;
                    Session["EmailCustomer"] = order.Email;

                    if (order.PromotionId != null)
                    {
                        order.TotalAmount = cart.Items.Sum(x => (x.Quantity * x.Price)) - order.DiscountAmount;
                    }
                    if (order.PromotionId == null)
                    {
                        order.TotalAmount = cart.Items.Sum(x => (x.Quantity * x.Price));
                    }

                    order.TypePayment = request.TypePayment;
                    order.CreateBy = request.Phone;

                    // Cộng 14 giờ do khi publish thì sẽ bị lệch múi giờ
                    order.ModifierDate = DateTime.Now;
                    // Tạo mã đơn hàng
                    order.CreateDate = DateTime.Now;

                    //Random rd = new Random();
                    //order.Code = "DH"+ rd.Next(0,9) + rd.Next(0,9) + rd.Next(0, 9) + rd.Next(0, 9);

                    ////////// Tạo mã đơn hàng với Ngày/Tháng/Năm //////////////
                    // Lấy ngày hiện tại dưới dạng ddMMyy
                    string currentDate = DateTime.Now.ToString("ddMMyy");
                    // Lấy ra tất cả các mã đơn hàng trong ngày hiện tại
                    var orderCodes = db.Orders
                        .Where(o => o.Code.StartsWith("DH" + currentDate))
                        .Select(o => o.Code)
                        .ToList();
                    // Tìm số đơn hàng cuối cùng trong ngày
                    int lastOrderNumber = 0;
                    foreach (var newCode in orderCodes)
                    {
                        int orderNumber;
                        if (int.TryParse(newCode.Substring(8), out orderNumber))
                        {
                            if (orderNumber > lastOrderNumber)
                            {
                                lastOrderNumber = orderNumber;
                            }
                        }
                    }
                    // Tạo mã đơn hàng mới
                    string newOrderCode = $"DH{currentDate}{(lastOrderNumber + 1).ToString("D5")}"; // D5 có nghĩa là 5 số 0 cuối, rồi + lên
                                                                                                    ////////// End Tạo mã đơn hàng với Ngày/Tháng/Năm //////////////


                    // Gán mã đơn hàng và lưu vào cơ sở dữ liệu
                    order.Code = newOrderCode;
                    db.Orders.Add(order);
                    db.SaveChanges();

                    //// Send Mail cho Khách hàng ////

                    var strSanPham = "";
                    var thanhTien = decimal.Zero;
                    var tongTien = decimal.Zero;
                    var khuyenMai = decimal.Zero;
                    string stringSubTotal = null;
                    foreach (var sp in cart.Items)
                    {
                        strSanPham += "<tr>";
                        strSanPham += "<td>" + sp.ProductName + "</td>";
                        strSanPham += "<td>" + sp.Quantity + "</td>";
                        strSanPham += "<td>" + Common.FormatNumber(sp.Price, 0) + "</td>";
                        strSanPham += "</tr>";

                        thanhTien += sp.Price * sp.Quantity;
                        stringSubTotal = thanhTien.ToString();
                    }

                    khuyenMai = order.DiscountAmount;

                    if (order.PromotionId != null)
                    {
                        tongTien = thanhTien - khuyenMai;
                    }

                    if (order.PromotionId == null)
                    {
                        tongTien = thanhTien;
                    }

                    tongTien = tongTien + phiShip;

                    var stringTotal = tongTien.ToString();
                    var trangThaiDon = string.Empty;
                    var hinhThucThanhToan = string.Empty;
                    string contentCustomer = "";

                    if (order.TypePayment == 3)
                    {
                        trangThaiDon = "<p>Trạng thái đơn hàng: <strong style=\"color:red;\">Chưa thanh toán</strong></p>\r\n";
                        hinhThucThanhToan = "PayPal";
                        contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/sendMailKhachHang.html"));

                    }
                    else if (order.TypePayment == 2)
                    {
                        trangThaiDon = "<p>Trạng thái đơn hàng: <strong style=\"color:red;\">Chưa thanh toán</strong></p>\r\n";
                        hinhThucThanhToan = "VNPAY";
                        contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/sendMailKhachHang.html"));
                    }
                    else
                    {
                        trangThaiDon = "<strong style=\"color:red;\">Chưa xác nhận</strong>";
                        hinhThucThanhToan = "COD";
                        contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/sendMailKhachHang_Offline.html"));
                    }

                    contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code);
                    contentCustomer = contentCustomer.Replace("{{TrangThaiDon}}", trangThaiDon);
                    contentCustomer = contentCustomer.Replace("{{HinhThucThanhToan}}", hinhThucThanhToan);
                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", order.CreateDate.ToString());
                    contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.CustomerName);
                    contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
                    contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
                    contentCustomer = contentCustomer.Replace("{{Email}}", order.Email);
                    contentCustomer = contentCustomer.Replace("{{GhiChu}}", order.Notes);
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                    contentCustomer = contentCustomer.Replace("{{ThanhTien}}", Common.FormatNumber(thanhTien, 0));
                    contentCustomer = contentCustomer.Replace("{{PhiShip}}", Common.FormatNumber(phiShip, 0));
                    contentCustomer = contentCustomer.Replace("{{KhuyenMai}}", Common.FormatNumber(khuyenMai, 0));
                    contentCustomer = contentCustomer.Replace("{{TongTien}}", Common.FormatNumber(tongTien, 0));
                    Common.SendMail(Message.Brand.ToString(), "Đơn hàng #" + order.Code, contentCustomer.ToString(), request.Email);


                    string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/sendMailCuaHang.html"));
                    contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code);
                    contentAdmin = contentAdmin.Replace("{{TrangThaiDon}}", trangThaiDon);
                    contentAdmin = contentAdmin.Replace("{{HinhThucThanhToan}}", hinhThucThanhToan);
                    contentAdmin = contentAdmin.Replace("{{NgayDat}}", order.CreateDate.ToString());
                    contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.CustomerName);
                    contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
                    contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
                    contentAdmin = contentAdmin.Replace("{{Email}}", order.Email);
                    contentAdmin = contentAdmin.Replace("{{GhiChu}}", order.Notes);
                    contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                    contentAdmin = contentAdmin.Replace("{{ThanhTien}}", Common.FormatNumber(thanhTien, 0));
                    contentAdmin = contentAdmin.Replace("{{PhiShip}}", Common.FormatNumber(phiShip, 0));
                    contentAdmin = contentAdmin.Replace("{{KhuyenMai}}", Common.FormatNumber(khuyenMai, 0));
                    contentAdmin = contentAdmin.Replace("{{TongTien}}", Common.FormatNumber(tongTien, 0));
                    Common.SendMail(Message.Brand.ToString(), "Đơn hàng mới #" + order.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["Email"]);
                    cart.ClearCart();
                    //// End Mail ////
                    code = new { Success = true, Code = request.TypePayment, Url = "" };


                    if (order.TypePayment == 2)
                    {
                        var url = UrlPayment(request.TypePaymentVN, order.Code);
                        code = new { Success = true, Code = order.TypePayment, Url = url };

                    }
                    if (order.TypePayment == 3)
                    {
                        Session["CurrentOrder"] = new { Order = order, Products = cart.Items, OrderCode = order.Code, Total = stringTotal, SubTotal = stringSubTotal };
                        code = new { Success = true, Code = order.TypePayment, Url = "/ShoppingCart/PaymentWithPaypal" };

                    }
                    Session["MaDonHang"] = order.Code;
                    Session["MaId"] = order.Id;
                }
            }

            return Json(code);
        }
        public ActionResult ConfirmMail()
        {
            int id = (int)Session["MaId"];
            string code = (string)Session["MaDonHang"];

            Session["MaId"] = null;
            Session["MaDonHang"] = null;

            ViewBag.CodeDonHang = code;
            var item = db.Orders.Find(id);
            if (item != null)
            {

                //// Send Mail cho Khách hàng ////

                var strSanPham = "";
                var thanhTien = decimal.Zero;
                var tongTien = decimal.Zero;
                var khuyenMai = decimal.Zero;
                string stringSubTotal = null;

                var itemDetail = db.OrderDetails.Where(s => s.OrderID == item.Id).ToList();

                foreach (var sp in itemDetail)
                {
                    strSanPham += "<tr>";
                    strSanPham += "<td>" + sp.Product.Title + "</td>";
                    strSanPham += "<td>" + sp.Quantity + "</td>";
                    strSanPham += "<td>" + Common.FormatNumber(sp.Price, 0) + "</td>";
                    strSanPham += "</tr>";

                    thanhTien += sp.Price * sp.Quantity;
                    stringSubTotal = thanhTien.ToString();
                }

                khuyenMai = item.DiscountAmount;

                if (item.PromotionId != null)
                {
                    tongTien = thanhTien - khuyenMai;
                }

                if (item.PromotionId == null)
                {
                    tongTien = thanhTien;
                }
                tongTien = tongTien + phiShip;
                var trangThaiDon = string.Empty;
                var hinhThucThanhToan = string.Empty;

                //item.Status = 1;
                item.IsConfirm = true;

                if (item.IsConfirm == true)
                {
                    trangThaiDon = "<p>Trạng thái đơn hàng: <strong style=\"color:green;\">Đã xác nhận</strong></p>\r\n<p style=\"margin:0 0 16px\">\r\nChúng tôi đang tiến hành hoàn thiện đơn\r\nđặt hàng của bạn\r\n</p>";
                    hinhThucThanhToan = "COD";
                }

                string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/sendMailKhachHang.html"));
                contentCustomer = contentCustomer.Replace("{{MaDon}}", item.Code);
                contentCustomer = contentCustomer.Replace("{{TrangThaiDon}}", trangThaiDon);
                contentCustomer = contentCustomer.Replace("{{HinhThucThanhToan}}", hinhThucThanhToan);
                contentCustomer = contentCustomer.Replace("{{NgayDat}}", item.CreateDate.ToString());
                contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", item.CustomerName);
                contentCustomer = contentCustomer.Replace("{{Phone}}", item.Phone);
                contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", item.Address);
                contentCustomer = contentCustomer.Replace("{{Email}}", item.Email);
                contentCustomer = contentCustomer.Replace("{{GhiChu}}", item.Notes);
                contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                contentCustomer = contentCustomer.Replace("{{ThanhTien}}", Common.FormatNumber(thanhTien, 0));
                contentCustomer = contentCustomer.Replace("{{PhiShip}}", Common.FormatNumber(phiShip, 0));
                contentCustomer = contentCustomer.Replace("{{KhuyenMai}}", Common.FormatNumber(khuyenMai, 0));
                contentCustomer = contentCustomer.Replace("{{TongTien}}", Common.FormatNumber(tongTien, 0));
                Common.SendMail(Message.Brand.ToString(), "Đơn hàng #" + item.Code, contentCustomer.ToString(), item.Email);

                string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/sendMailCuaHang.html"));
                contentAdmin = contentAdmin.Replace("{{MaDon}}", item.Code);
                contentAdmin = contentAdmin.Replace("{{TrangThaiDon}}", trangThaiDon);
                contentAdmin = contentAdmin.Replace("{{HinhThucThanhToan}}", hinhThucThanhToan);
                contentAdmin = contentAdmin.Replace("{{NgayDat}}", item.CreateDate.ToString());
                contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", item.CustomerName);
                contentAdmin = contentAdmin.Replace("{{Phone}}", item.Phone);
                contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", item.Address);
                contentAdmin = contentAdmin.Replace("{{Email}}", item.Email);
                contentAdmin = contentAdmin.Replace("{{GhiChu}}", item.Notes);
                contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                contentAdmin = contentAdmin.Replace("{{ThanhTien}}", Common.FormatNumber(thanhTien, 0));
                contentAdmin = contentAdmin.Replace("{{PhiShip}}", Common.FormatNumber(phiShip, 0));
                contentAdmin = contentAdmin.Replace("{{KhuyenMai}}", Common.FormatNumber(khuyenMai, 0));
                contentAdmin = contentAdmin.Replace("{{TongTien}}", Common.FormatNumber(tongTien, 0));
                Common.SendMail(Message.Brand.ToString(), "Đơn hàng mới #" + item.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["Email"]);

                // Điều phối đơn xuống cửa hàng
                int storeID = StoreSingleton.Instance.Id; // Lấy ra id của cửa hàng mà khách hàng chọn
                item.StoreID = storeID;
                item.Status = 6;

                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return View();
        }

        public ActionResult Partial_Item_ThanhToan()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            decimal phiShipThanhToan = phiShip;
            TempData["PhiShip"] = phiShipThanhToan;
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        public ActionResult Partial_Item_Cart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        public ActionResult Partial_List_PromotionCode()
        {
            var items = db.Promotions.Where(x => x.IsActive).OrderByDescending(x => x.Id).ToList();
            return PartialView(items);
        }

        [HttpPost]
        public ActionResult KiemTraVaApDungMaKhuyenMai(string maKhuyenMai)
        {
            // Thực hiện kiểm tra mã khuyến mãi và tính toán giảm giá
            // Trả về kết quả dưới dạng JSON, ví dụ: { success: true, tongTienCartFormatted: "1,000,000 VND" }
            var promotion = db.Promotions.FirstOrDefault(p => p.PromotionCode == maKhuyenMai && p.IsActive);

            var userId = User.Identity.GetUserId();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = userManager.FindById(userId);

            var userCheckPoint = user.CheckPoint;

            if (promotion != null)
            {

                if (User.Identity.IsAuthenticated)
                {
                    if (promotion.Point > userCheckPoint)
                    {
                        userCheckPoint -= promotion.Point;
                        user.CheckPoint = userCheckPoint;
                        return Json(new { success = false });

                    }
                    else
                    {
                        var promotionID = promotion.Id;
                    }
                }

            }
            // Kiểm tra nếu không tìm thấy mã khuyến mãi
            if (promotion == null)
            {
                return Json(new { success = false });
            }
            // Nếu mã khuyến mãi hợp lệ, tính toán lại tổng tiền của giỏ hàng và định dạng nó cho phù hợp
            var tongTienCart = TinhToanLaiTongTienGioHang(maKhuyenMai);


            var tongTienCartFormatted = Common.FormatNumber(tongTienCart, 0);

            // Trả về kết quả dưới dạng JSON
            return Json(new { success = true, tongTienCartFormatted });
        }

        private decimal TinhToanLaiTongTienGioHang(string maKhuyenMai)
        {
            decimal tongTienCart = 0;
            // Lấy giỏ hàng từ Session
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                // Tính toán tổng tiền trước khi áp dụng mã khuyến mãi
                tongTienCart = cart.Items.Sum(item => item.TotalPrice);

                // Kiểm tra và áp dụng mã khuyến mãi nếu hợp lệ
                if (!string.IsNullOrEmpty(maKhuyenMai))
                {
                    var promotion = db.Promotions.FirstOrDefault(p => p.PromotionCode == maKhuyenMai && p.IsActive);
                    var promotionID = promotion.Id;

                    if (promotion != null)
                    {

                        if (promotion.TypePromotion == 1)
                        {
                            // Loại 1: Trừ một số tiền cụ thể
                            decimal giamGia = promotion.DiscountAmount;
                            int typePromotion = promotion.TypePromotion;

                            cart.TypePromotion = typePromotion;
                            cart.DiscountAmount = giamGia;
                            TempData["DiscountAmount"] = cart.DiscountAmount;

                            // Áp dụng giảm giá vào tổng tiền
                            tongTienCart -= giamGia;

                            // Đảm bảo tổng tiền không nhỏ hơn 0
                            if (tongTienCart < 0)
                            {
                                tongTienCart = 0;
                            }

                        }
                        else if (promotion.TypePromotion == 2)
                        {
                            // Loại 2: Giảm giá dựa trên phần trăm
                            decimal phanTramGiamGia = promotion.DiscountAmount;

                            // Tính số tiền giảm dựa trên phần trăm và áp dụng giảm giá vào tổng tiền
                            decimal giamGia = tongTienCart * phanTramGiamGia;
                            int typePromotion = promotion.TypePromotion;
                            cart.DiscountAmount = giamGia;
                            cart.TypePromotion = typePromotion;

                            TempData["DiscountAmount"] = cart.DiscountAmount;

                            tongTienCart -= giamGia;

                        }
                        cart.PromotionId = promotionID;
                        cart.PromotionCode = maKhuyenMai;

                        TempData["IDMaKhuyenMai"] = cart.PromotionId;
                        TempData["MaKhuyenMai"] = cart.PromotionCode;
                    }
                }
            }
            TempData["TongTienCartKM"] = tongTienCart;
            return tongTienCart;
        }

        private decimal KiemTraVaLayGiamGiaTuMaKhuyenMai(string maKhuyenMai)
        {
            // Thực hiện kiểm tra và lấy giảm giá từ mã khuyến mãi
            // Trong ví dụ này, bạn có thể xử lý logic kiểm tra mã khuyến mãi trong cơ sở dữ liệu hoặc bất kỳ cách nào bạn muốn để lấy giảm giá tương ứng.
            // Nếu mã khuyến mãi hợp lệ, trả về giá trị giảm giá, nếu không, trả về 0.

            // Ví dụ: kiểm tra mã khuyến mãi trong cơ sở dữ liệu
            var giamGia = db.Promotions.FirstOrDefault(p => p.PromotionCode == maKhuyenMai);

            if (giamGia != null)
            {
                return giamGia.DiscountAmount;
            }

            return 0; // Trả về 0 nếu mã khuyến mãi không hợp lệ hoặc không tìm thấy
        }



        public ActionResult ShowCount() // Hàm dùng để lưu Session Cart
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count, JsonRequestBehavior.AllowGet });
            }
            return Json(new { Count = 0, JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            var code = new { success = false, message = Message.NoMessage.ToString(), code = -1, Count = 0}; // Giá trị ban đầu
            var checkProduct = db.Products.FirstOrDefault(x => x.Id == id);
            if (checkProduct != null)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                // Kiểm tra xem trong Cart đã có sản phẩm và có StoreID != 0 chưa
                if (cart.Items.Any(item => item.ProductStoreID != 0 && item.ProductStoreID != checkProduct.StoreID && checkProduct.StoreID.HasValue))
                {

                    // Nếu đã có sản phẩm có StoreID khác 0 trong Cart, có thể xử lý thông báo hoặc thực hiện các bước khác tùy thuộc vào yêu cầu của bạn.
                    code = new { success = false, message = "Không thể thêm sản phẩm giới hạn từ 2 cừa hàng khác nhau", code = -1, Count = cart.Items.Count };
                    return Json(code);
                }
                else
                {
                    var cartItemsExists = cart.Items.FirstOrDefault(x => x.ProductId == id);
                    if (cartItemsExists != null)
                    {
                        // Có thể xử lý trường hợp sản phẩm đã tồn tại trong giỏ hàng ở đây nếu cần
                    }
                    ShoppingCartItem item = new ShoppingCartItem
                    {
                        ProductId = checkProduct.Id,
                        Alias = checkProduct.Alias,
                        ProductName = checkProduct.Title,
                        CategoryName = checkProduct.ProductCategory.Title,
                        Quantity = quantity,
                        ProductStoreID = checkProduct.StoreID.HasValue ? checkProduct.StoreID.Value : 0 // Giá trị mặc định khi không có StoreID
                    };
                    if (checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                    {
                        item.ProductImg = checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault).Image;
                    }
                    item.Price = checkProduct.Price;
                    if (checkProduct.PriceSale > 0)  // nếu sản phẩm có giảm giá, thì lấy giá tiền = giá đã giảm
                    {
                        item.Price = (decimal)checkProduct.PriceSale;
                    }
                    item.TotalPrice = item.Quantity * item.Price;
                    cart.AddToCart(item, quantity); // Hàm AddToCart này bên class Shoping Cart
                    Session["Cart"] = cart; // Khi thành công thì lưu lại Session
                    code = new { success = true, message = Message.SuccessAddToCart.ToString(), code = 1, Count = cart.Items.Count }; // Thành công thì in ra thông báo
                }
            }
            return Json(code);
        }

        [HttpGet]
        public ActionResult GetCartJson()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            return Json(cart, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SetCartInSession(ShoppingCart cartData)
        {
            Session["Cart"] = cartData;
            return Json(new { success = true});

        }


        [HttpPost]
        public ActionResult DeleteCartItem(int id)
        {
            var code = new { success = false, code = -1, Count = 0 };
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProductExists = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (checkProductExists != null)
                {
                    cart.Items.Remove(checkProductExists);
                    TempData.Remove("TongTienCartKM");
                    TempData.Remove("IDMaKhuyenMai");
                    TempData.Remove("MaKhuyenMai");
                    TempData.Remove("DiscountAmount");
                    return Json(new { success = true, code = 1, Count = cart.Items.Count });
                }
            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult UpdateQuantityCartItem(int id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.UpdateQuantity(id, quantity);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteAllCart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.Items.Clear();
                TempData.Remove("TongTienCartKM");
                TempData.Remove("IDMaKhuyenMai");
                TempData.Remove("MaKhuyenMai");
                TempData.Remove("DiscountAmount");
                return Json(new { success = true });
            }
            return Json(new { success = false });

        }


        /// <summary>
        /// Thanh toán VNPAY
        /// </summary>
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.Orders.FirstOrDefault(x => x.Code == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = ((long)order.TotalAmount + (long)phiShip) * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.CreateDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }

        public ActionResult PayPalPayment_SuccessView()
        {
            string email = TempData["EmailCustomerPaypal"] as string;

            return View();
        }

        public ActionResult PayPalPayment_FailureView()
        {
            return View();
        }

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            var orderCode = TempData["OrderCodePayPal"];

            var order = db.Orders.ToList().Where(x => x.Code == orderCode).FirstOrDefault();
            var orderEmail = TempData["EmailCustomerPaypal"];
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/shoppingcart/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("PayPalPayment_FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("PayPalPayment_FailureView");
            }
            var itemOrder = db.Orders.FirstOrDefault(x => x.Code == orderCode);
            if (itemOrder != null)
            {
                itemOrder.Status = 2;//đã thanh toán
                itemOrder.IsConfirm = true;


                // Điều phối đơn xuống cửa hàng
                int storeID = StoreSingleton.Instance.Id; // Lấy ra id của cửa hàng mà khách hàng chọn
                itemOrder.StoreID = storeID;
                itemOrder.Status = 6;

                //if (itemOrder.StoreID != null)
                //{
                //    itemOrder.Status = 6;
                //}

                db.Orders.Attach(itemOrder);
                db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //// Send Mail cho Khách hàng ////
                var item = db.OrderDetails.Where(s => s.OrderID == itemOrder.Id).ToList();

                var strSanPham = "";
                var thanhTien = decimal.Zero;
                var tongTien = decimal.Zero;
                var khuyenMai = decimal.Zero;
                string stringSubTotal = null;
                foreach (var sp in item)
                {
                    strSanPham += "<tr>";
                    strSanPham += "<td>" + sp.Product.Title + "</td>";
                    strSanPham += "<td>" + sp.Quantity + "</td>";
                    strSanPham += "<td>" + Common.FormatNumber(sp.Price, 0) + "</td>";
                    strSanPham += "</tr>";

                    thanhTien += sp.Price * sp.Quantity;
                    stringSubTotal = thanhTien.ToString();
                }

                khuyenMai = itemOrder.DiscountAmount;

                if (itemOrder.PromotionId != null)
                {
                    tongTien = thanhTien - khuyenMai;
                }

                if (itemOrder.PromotionId == null)
                {
                    tongTien = thanhTien;
                }

                tongTien = tongTien + phiShip;

                var trangThaiDon = string.Empty;
                var hinhThucThanhToan = string.Empty;

                if (itemOrder.Status == 2 || itemOrder.Status == 6)
                {
                    trangThaiDon = "<p>Trạng thái đơn hàng: <strong style=\"color:green;\">Đã thanh toán</strong></p>\r\n<p style=\"margin:0 0 16px\">\r\nChúng tôi đang tiến hành hoàn thiện đơn\r\nđặt hàng của bạn\r\n</p>";
                    hinhThucThanhToan = "PayPal";
                }
                string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/sendMailKhachHang.html"));
                contentCustomer = contentCustomer.Replace("{{MaDon}}", itemOrder.Code);
                contentCustomer = contentCustomer.Replace("{{TrangThaiDon}}", trangThaiDon);
                contentCustomer = contentCustomer.Replace("{{GhiChu}}", itemOrder.Notes);
                contentCustomer = contentCustomer.Replace("{{PhiShip}}", Common.FormatNumber(phiShip, 0));
                contentCustomer = contentCustomer.Replace("{{HinhThucThanhToan}}", hinhThucThanhToan);
                contentCustomer = contentCustomer.Replace("{{NgayDat}}", itemOrder.CreateDate.ToString());
                contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", itemOrder.CustomerName);
                contentCustomer = contentCustomer.Replace("{{Phone}}", itemOrder.Phone);
                contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", itemOrder.Address);
                contentCustomer = contentCustomer.Replace("{{Email}}", itemOrder.Email);
                contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                contentCustomer = contentCustomer.Replace("{{ThanhTien}}", Common.FormatNumber(thanhTien, 0));
                contentCustomer = contentCustomer.Replace("{{KhuyenMai}}", Common.FormatNumber(khuyenMai, 0));
                contentCustomer = contentCustomer.Replace("{{TongTien}}", Common.FormatNumber(tongTien, 0));
                Common.SendMail(Message.Brand.ToString(), "Đơn hàng #" + itemOrder.Code, contentCustomer.ToString(), itemOrder.Email);
            }
            ////on successful payment, show success page to user.  
            return View("PayPalPayment_SuccessView");
        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {

            var currentOrder = (dynamic)Session["CurrentOrder"];
            var orderCode = currentOrder.OrderCode;
            var totalAmount = currentOrder.Total;
            var subTotal = currentOrder.SubTotal;

            phiShip = Math.Round(phiShip / 25000, 2);

            var order = db.Orders.ToList().Where(x => x.Code == orderCode).FirstOrDefault();
            var subtotalValue = (phiShip + Math.Round(order.TotalAmount / 25000, 2)).ToString();

            TempData["OrderCodePayPal"] = order.Code;
            TempData["EmailCustomerPaypal"] = order.Email;
            TempData["MaDonHang"] = order.Code;


            var itemList = new ItemList()
            {
                items = new List<Item>()
            };


            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = order.Code,
                currency = "USD",
                price = subtotalValue,
                quantity = "1",
                sku = "1"
            });

            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = subtotalValue
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = (Convert.ToDecimal(details.tax) + Convert.ToDecimal(details.shipping) + Convert.ToDecimal(details.subtotal)).ToString(), // Cập nhật total
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }
    }
}