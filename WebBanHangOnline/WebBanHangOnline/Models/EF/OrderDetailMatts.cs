using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_OrderDetailMatts")]
    public class OrderDetailMatts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        public int OrderMattsID { get; set; }
        public int MatterialID { get; set; }
        public int Quantity { get; set; }

        public virtual OrderMatts OrderMatts { get; set; }
        public virtual Matterial Matterial { get; set; }
    }
}