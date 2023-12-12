using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models
{
    public class ProductBoughtTogetherViewModel
    {
        public int ProductID1 { get; set; }
        public string ProductName1 { get; set; }
        public int ProductID2 { get; set; }
        public string ProductName2 { get; set; }
        public int BuyCount { get; set; }
    }
}