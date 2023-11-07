using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_CustomerRequest")]
    public class CustomerRequest
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RequestTitle { get; set; }
        [AllowHtml]
        public string RequestContent { get; set; }
        public int RequestTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsResolve { get; set; }
        public virtual RequestType RequestType { get; set; }
    }
}