using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Store")]
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }
    }
}