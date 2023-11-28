using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Models
{
    public class StrategyForAdmin : CommonAbstract
    {
        public override void SetCreated()
        {

        }

        public override void SetModified()
        {

        }
        public override void SetCreatedBy()
        {
            this.CreateBy = "Admin";
        }

        public override void SetModifiedBy()
        {
            this.ModifierBy = "Admin";
        }
    }
}