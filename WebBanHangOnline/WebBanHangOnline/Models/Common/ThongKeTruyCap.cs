using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace WebBanHangOnline.Models.Common
{
    // Dùng để gọi Stored Procedure
    public class ThongKeTruyCap
    {
        public static string strConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public static  ThongKeViewModel ThongKe()
        {
            using (var connect = new SqlConnection(strConnection))
            {
                var item = connect.QueryFirstOrDefault<ThongKeViewModel>("sp_ThongKe",commandType:CommandType.StoredProcedure);
                return item;
            }
        }
    }
}