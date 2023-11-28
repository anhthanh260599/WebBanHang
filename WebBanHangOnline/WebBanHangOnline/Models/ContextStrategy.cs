using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Models
{
    public class ContextStrategy
    {
        public string ModifierBy { get; set; }
        public string CreateBy { get; set; }
        private CommonAbstract request;

        public ContextStrategy(CommonAbstract strategy)
        {
            this.request = strategy;
        }

        // Phương thức để thay đổi chiến lược tại thời điểm chạy
        public void SetStrategy(CommonAbstract strategy)
        {
            this.request = strategy;
        }

        //Phương thức sử dụng chiến lược
        public void ExecuteCreate()
        {
            request.SetCreatedBy();
        }

        public void ExecuteModify()
        {
            request.SetModifiedBy();
        }
    }
}