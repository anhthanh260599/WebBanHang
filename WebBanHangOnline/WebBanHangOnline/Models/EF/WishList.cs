using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_WishList")]
    public class WishList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("User")]
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual ApplicationUser User { get; set; } 
        public virtual Product Products { get; set; }
    }
}