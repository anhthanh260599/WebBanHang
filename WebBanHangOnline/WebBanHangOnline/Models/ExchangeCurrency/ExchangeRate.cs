using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.ExchangeCurrency
{

    // Sử dụng API: "https://open.er-api.com/v6/latest/USD"
    // Lưu ý: Khai báo fields nhớ giống với các Key ở link API
    public class ExchangeRate
    {
        public string Result { get; set; }
        public string Provider { get; set; }
        public string Documentation { get; set; }
        public string Terms_Of_Use { get; set; }
        public long Time_Last_Update_Unix { get; set; }
        public DateTime Time_Last_Update_Utc { get; set; }
        public long Time_Next_Update_Unix { get; set; }
        public DateTime Time_Next_Update_Utc { get; set; }
        public long Time_Eol_Unix { get; set; }
        public string Base_Code { get; set; }
        public Rates Rates { get; set; }
    }
}