using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.Common;
using WebBanHangOnline.Models.EF;

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
            var code = new { success = false, Code = -1 };
            if(ModelState.IsValid)
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
                    order.TotalAmount = cart.Items.Sum(x=> (x.Quantity * x.Price));
                    order.TypePayment = request.TypePayment;
                    order.CreateBy = request.Phone;
                    // Tạo mã đơn hàng
                    order.CreateDate = DateTime.Now;
                    order.ModifierDate = DateTime.Now;
                    Random rd = new Random();
                    order.Code = "DH"+ rd.Next(0,9) + rd.Next(0,9) + rd.Next(0, 9) + rd.Next(0, 9);
                    db.Orders.Add(order);
                    db.SaveChanges();
                    cart.ClearCart();
                    return RedirectToAction("CheckoutSuccess");
                }
            }
            return Json(request);
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
    }
}