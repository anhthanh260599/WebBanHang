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

namespace WebBanHangOnline.Controllers
{
    public class ShoppingCartController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
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

        public ActionResult VnPayReturn()
        {
            //var a = UrlPayment(0, "DH02092300008");

            return View();
        }

        public ActionResult CheckoutSuccess()
        {
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
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(OrderViewModel request)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    Order order = new Order();
                    order.CustomerName = request.CustomerName;
                    order.Address = request.Address;
                    order.Phone = request.Phone;
                    cart.Items.ForEach(x => order.OrderDetails.Add(new OrderDetail
                    {
                        ProductID = x.ProductId,
                        Price = x.Price,
                        Quantity = x.Quantity

                    }));
                    order.Quantity = cart.Items.Sum(x=>x.Quantity);
                    order.Email = request.Email;
                    order.TotalAmount = cart.Items.Sum(x=> (x.Quantity * x.Price));
                    order.TypePayment = request.TypePayment;
                    order.CreateBy = request.Phone;
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

                    foreach (var sp in cart.Items)
                    {
                        strSanPham += "<tr>";
                        strSanPham += "<td>" + sp.ProductName + "</td>";
                        strSanPham += "<td>" + sp.Quantity + "</td>";
                        strSanPham += "<td>" + Common.FormatNumber(sp.Price,0) + "</td>";
                        strSanPham += "</tr>";

                        thanhTien += sp.Price * sp.Quantity;

                    }
                    tongTien = thanhTien;
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/sendMailKhachHang.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDon}}",order.Code);
                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", order.CreateDate.ToString());
                    contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.CustomerName);
                    contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
                    contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
                    contentCustomer = contentCustomer.Replace("{{Email}}", order.Email);
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                    contentCustomer = contentCustomer.Replace("{{ThanhTien}}", Common.FormatNumber(thanhTien,0));
                    contentCustomer = contentCustomer.Replace("{{TongTien}}", Common.FormatNumber(tongTien,0));
                    Common.SendMail("Cà phê Nhóm 4", "Đơn hàng #" + order.Code, contentCustomer.ToString(), request.Email);

                    string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/sendMailCuaHang.html"));
                    contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code);
                    contentAdmin = contentAdmin.Replace("{{NgayDat}}", order.CreateDate.ToString());
                    contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.CustomerName);
                    contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
                    contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
                    contentAdmin = contentAdmin.Replace("{{Email}}", order.Email);
                    contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                    contentAdmin = contentAdmin.Replace("{{ThanhTien}}", Common.FormatNumber(thanhTien, 0));
                    contentAdmin = contentAdmin.Replace("{{TongTien}}", Common.FormatNumber(tongTien, 0));
                    Common.SendMail("Cà phê Nhóm 4", "Đơn hàng mới #" + order.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["Email"]);
                    cart.ClearCart();
                    //// End Mail ////
                    code = new { Success = true, Code = request.TypePayment, Url = "" };
                    //var url =     
                    if (request.TypePayment == 2)
                    {
                        var url = UrlPayment(request.TypePaymentVN, order.Code);
                        code = new { Success = true, Code = request.TypePayment, Url = url };
                    }   
                    //code = new {success = true, Code = 1, Url= Url };
                    //return RedirectToAction("CheckoutSuccess");
                }
            }
            return Json(code);
        }

        public ActionResult Partial_Item_ThanhToan()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
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
            //Message messages = new Message();
            var code = new { success = false, message = Message.NoMessage.ToString() , code=-1 , Count = 0}; // Giá trị ban đầu
            var checkProduct = db.Products.FirstOrDefault(x=>x.Id == id);
            if (checkProduct != null)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if(cart == null)
                {
                    cart = new ShoppingCart();  
                }
                var cartItemsExists = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (cartItemsExists != null)
                {

                }
                ShoppingCartItem item = new ShoppingCartItem
                {
                    ProductId = checkProduct.Id,
                    Alias = checkProduct.Alias,
                    ProductName = checkProduct.Title,
                    CategoryName = checkProduct.ProductCategory.Title,
                    Quantity = quantity
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
                code = new { success = true, message = Message.SuccessAddToCart.ToString(), code = 1 , Count= cart.Items.Count }; // Thành công thì in ra thông báo
            }
            return Json(code);
        }

     

        [HttpPost]
        public ActionResult DeleteCartItem(int id)
        {
            var code = new { success = false,  code = -1, Count = 0 };
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if(cart != null)
            {
                var checkProductExists = cart.Items.FirstOrDefault(x=>x.ProductId == id);
                if (checkProductExists != null)
                {
                    cart.Items.Remove(checkProductExists);
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
            if(cart != null )
            {
                cart.Items.Clear();
                return Json(new {success = true});
            }
            return Json(new { success = false });

        }


        /// <summary>
        /// Thanh toán VNPAY
        /// </summary>
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.Orders.FirstOrDefault(x=>x.Code == orderCode);

            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            ////Get payment input
            //OrderInfo order = new OrderInfo();
            //order.OrderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            //order.Amount = 100000; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            //order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending" khởi tạo giao dịch chưa có IPN
            //order.CreatedDate = DateTime.Now;
            //Save order to db

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalAmount * 100; // Parse lại để bỏ 2 số 00 sau dấu "." thì mới hoạt động được
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
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán dơn hàng: " + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            //Response.Redirect(paymentUrl);
            return urlPayment;
        }
    }
}