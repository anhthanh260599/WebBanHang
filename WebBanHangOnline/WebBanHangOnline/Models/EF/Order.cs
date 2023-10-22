﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebBanHangOnline.Models.Common;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Order")]
    public class Order : CommonAbstract
    {
        public Order() 
        {
            this.OrderDetails = new HashSet<OrderDetail>(); // quan hệ 1 - nhiều
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng" )]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập địa chỉ")]
        public string Address { get; set; }
        public string Email { get; set; }
        public bool IsConfirm { get; set; }

        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
        public int TypePayment { get; set; }
        public int Status { get; set; }
        public string CustomerID { get; set; }
        public int? PromotionId { get; set; }
        public string PromotionCode { get; set; } 
        public decimal DiscountAmount { get; set; } 
        public int TypePromotion { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } // quan hệ 1 - nhiều
        public virtual Promotion Promotion { get; set; }
    }
}