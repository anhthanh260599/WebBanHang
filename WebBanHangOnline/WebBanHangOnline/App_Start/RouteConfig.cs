﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebBanHangOnline
{
    /// <summary>
    /// Tạo route để chuyển trang, link ấy, thay vì hiển thị Product/Index là san-pham
    /// </summary>
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute( // Trang chủ
             name: "Home",
             url: "trang-chu",
             defaults: new { controller = "Home", action = "Index", alias = UrlParameter.Optional },
             namespaces: new[] { "WebBanHangOnline.Controllers" }
         );

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute( // Giỏ hàng
             name: "ShoppingCart",
             url: "gio-hang",
             defaults: new { controller = "ShoppingCart", action = "Index", alias = UrlParameter.Optional },
             namespaces: new[] { "WebBanHangOnline.Controllers" }
         );

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute( // Thanh toán
             name: "Checkout",
             url: "thanh-toan",
             defaults: new { controller = "ShoppingCart", action = "Checkout", alias = UrlParameter.Optional },
             namespaces: new[] { "WebBanHangOnline.Controllers" }
         );

            routes.MapRoute( // Thanh toán vnpay
              name: "vnpay_return",
              url: "vnpay_return",
              defaults: new { controller = "ShoppingCart", action = "VnPay_Return", alias = UrlParameter.Optional },
              namespaces: new[] { "WebBanHangOnline.Controllers" }
          );

            routes.MapRoute( // Trang danh mục sản phẩm
               name: "Products",
               url: "san-pham",
               defaults: new { controller = "Product", action = "Index", alias = UrlParameter.Optional },
               namespaces: new[] { "WebBanHangOnline.Controllers" }
           );

            routes.MapRoute( // Trang danh mục sản phẩm
               name: "DetailProduct",
               url: "chi-tiet/{alias}-p{id}",
               defaults: new { controller = "Product", action = "Detail", alias = UrlParameter.Optional },
               namespaces: new[] { "WebBanHangOnline.Controllers" }
           );

            routes.MapRoute( // Danh mục sản phẩm theo hãng
             name: "CategoryProduct",
             url: "san-pham/{alias}-{id}",
             defaults: new { controller = "Product", action = "ProductCategory", id = UrlParameter.Optional },
             namespaces: new[] { "WebBanHangOnline.Controllers" }
         );


            routes.MapRoute( // Trang tin tức
             name: "ListNews",
             url: "tin-tuc",
             defaults: new { controller = "News", action = "Index", alias = UrlParameter.Optional },
             namespaces: new[] { "WebBanHangOnline.Controllers" }
         );

            routes.MapRoute( // Trang chi tiết tin tức
             name: "DetailNews",
             url: "tin-tuc/chi-tiet/{alias}-n{id}",
             defaults: new { controller = "News", action = "Detail", id = UrlParameter.Optional },
             namespaces: new[] { "WebBanHangOnline.Controllers" }
         );

            routes.MapRoute( // Trang liên hệ
              name: "Contact",
              url: "lien-he",
              defaults: new { controller = "Contact", action = "Index", alias = UrlParameter.Optional },
              namespaces: new[] { "WebBanHangOnline.Controllers" }
          );

            routes.MapRoute( // Trang cửa hàng
           name: "Store",
           url: "cua-hang",
           defaults: new { controller = "Store", action = "Index", alias = UrlParameter.Optional },
           namespaces: new[] { "WebBanHangOnline.Controllers" }
       );

            routes.MapRoute( // Trang chi tiết cửa hàng
          name: "DetailStore",
          url: "cua-hang/chi-tiet/{alias}-s{id}",
          defaults: new { controller = "Store", action = "Detail", id = UrlParameter.Optional },
          namespaces: new[] { "WebBanHangOnline.Controllers" }
        );

            routes.MapRoute( // Trang bài viết khuyến mãi
           name: "Posts",
           url: "post/{alias}",
           defaults: new { controller = "Artical", action = "Index", alias = UrlParameter.Optional },
           namespaces: new[] { "WebBanHangOnline.Controllers" }
       );

            routes.MapRoute( // Trang tra cứu đơn hàng
              name: "TraCuu",
              url: "tra-cuu",
              defaults: new { controller = "Home", action = "TraCuuDonHang", alias = UrlParameter.Optional },
              namespaces: new[] { "WebBanHangOnline.Controllers" }
          );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "WebBanHangOnline.Controllers" }
            );
        }
    }
}
