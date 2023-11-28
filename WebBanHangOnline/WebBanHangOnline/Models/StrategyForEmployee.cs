using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Models
{
    public class StrategyForEmployee : CommonAbstract
    {
        public string ModifierBy { get; set; }
        public string CreateBy { get; set; }
        public override void SetCreated()
        {

        }

        public override void SetModified()
        {

        }
        public override void SetCreatedBy()
        {
            this.CreateBy = "Store";
        }

        public override void SetModifiedBy()
        {
            this.ModifierBy = "Store";
        }
    }
}