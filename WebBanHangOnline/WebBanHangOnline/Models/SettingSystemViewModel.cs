using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models
{
    public class SettingSystemViewModel
    {
        // Setting cái gì thì thêm fields đó, ví dụ setting slider hoặc banner thì thêm fields vô.
        public string SettingTitle { get; set; }
        public string SettingLogo { get; set; }
        public string SettingHotline { get; set; }
        public string SettingEmail { get; set; }
        public string SettingTitleSeo { get; set; }
        public string SettingDesSeo { get; set; }
        public string SettingKeySeo { get; set; }


    }
}