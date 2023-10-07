using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_OrderMatts")]
    public class OrderMatts : CommonAbstract
    {
        public OrderMatts()
        {
            this.OrderDetailMatts = new HashSet<OrderDetailMatts>(); // quan hệ 1 - nhiều
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        public int StoreID { get; set; }
        [Required]
        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public virtual Store Store { get; set; }
        public virtual ICollection<OrderDetailMatts> OrderDetailMatts { get; set; } // quan hệ 1 - nhiều

    }
}