using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebBanHangOnline.Models.Common;

namespace WebBanHangOnline
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Timer timer;

        protected void Application_Start()
        {
            timer = new Timer(ExecuteStoredProcedure, null, 0, 1000); // 10s thực hiện stored 1 lần

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application["HomNay"] = 0;
            Application["HomQua"] = 0;
            Application["TuanNay"] = 0;
            Application["TuanTruoc"] = 0;
            Application["ThangNay"] = 0;
            Application["ThangTruoc"] = 0;
            Application["TatCa"] = 0;
            Application["visitors_online"] = 0;
        }

        void Session_Start(object sender, EventArgs e)
        {
            Session.Timeout = 150;
            Application.Lock();
            Application["visitors_online"] = Convert.ToInt32(Application["visitors_online"]) + 1; // đếm lượt truy cập số người online
            Application.UnLock();
            try
            {
                var item = WebBanHangOnline.Models.Common.ThongKeTruyCap.ThongKe();
                if (item != null)
                {
                    Application["HomNay"] = long.Parse("0" + item.HomNay.ToString("#,###"));
                    Application["HomQua"] = long.Parse("0" + item.HomQua.ToString("#,###"));
                    Application["TuanNay"] = long.Parse("0" + item.TuanNay.ToString("#,###"));
                    Application["TuanTruoc"] = long.Parse("0" + item.TuanTruoc.ToString("#,###"));
                    Application["ThangNay"] = long.Parse("0" + item.ThangNay.ToString("#,###"));
                    Application["ThangTruoc"] = long.Parse("0" + item.ThangTruoc.ToString("#,###"));
                    Application["TatCa"] = ( int.Parse(item.TatCa.ToString())).ToString("#,###");
                }
            }
            catch { }
        }

        void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["visitors_online"] = Convert.ToInt32(Application["visitors_online"]) - 1; // đếm lượt truy cập số người online
            Application.UnLock();
        }

        private void ExecuteStoredProcedure(object state)
        {
            try
            {
                // Gọi stored procedure ở đây
                WebBanHangOnline.Models.Common.ExecuteStoredProcedure.UpdateStatusIfNoChange();
                WebBanHangOnline.Models.Common.ExecuteStoredProcedure.sp_UpdateIsActivePromotionCode();

                HttpContext.Current.Application["LastStoredProcedureExecution"] = DateTime.Now.AddHours(15); /// AddHours(14) nếu deploy 
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
            }
        }
    }
}
