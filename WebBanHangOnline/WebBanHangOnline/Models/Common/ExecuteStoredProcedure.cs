using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;

namespace WebBanHangOnline.Models.Common
{
    public class ExecuteStoredProcedure
    {
        public static string strConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public static ThongKeViewModel ThongKe()
        {
            using (var connect = new SqlConnection(strConnection))
            {
                var item = connect.QueryFirstOrDefault<ThongKeViewModel>("sp_ThongKe", commandType: CommandType.StoredProcedure);
                return item;
            }
        }

        public static void UpdateStatusIfNoChange()
        {
            using (SqlConnection connection = new SqlConnection(strConnection))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("UpdateStatusIfNoChange", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Thực thi stored procedure
                command.ExecuteNonQuery();
            }
        }
    }
}