using System;
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
             name: "News",
             url: "tin-tuc",
             defaults: new { controller = "News", action = "Index", alias = UrlParameter.Optional },
             namespaces: new[] { "WebBanHangOnline.Controllers" }
         );

            routes.MapRoute( // Trang liên hệ
              name: "Contact",
              url: "lien-he",
              defaults: new { controller = "Contact", action = "Index", alias = UrlParameter.Optional },
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
