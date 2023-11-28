using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models
{
    public abstract class CommonAbstract
    {
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifierBy { get; set; }
        public DateTime ModifierDate { get; set; }

        public abstract void SetCreated();

        public abstract void SetModified();

        public abstract void SetCreatedBy();

        public abstract void SetModifiedBy();
    }
}