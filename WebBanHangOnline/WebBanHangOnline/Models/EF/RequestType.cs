using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_RequestType")]
    public class RequestType
    {
        public int Id { get; set; }
        public string RequestTypeName { get; set; }
        public virtual ICollection<CustomerRequest> CustomerRequests { get; set; } // quan hệ 1 - nhiều

    }
}