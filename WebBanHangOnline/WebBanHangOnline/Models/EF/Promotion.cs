using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Promotion")]
    public class Promotion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string PromotionCode { get; set; }

        [Required]
        public string PromotionName { get; set; }

        [Required]
        public decimal DiscountAmount { get; set; }
        [Required]
        public DateTime DateFrom { get; set; }
        [Required]
        public DateTime DateTo { get; set; }
        public int Quantity { get; set; }
        public int TypePromotion { get; set; }
        public bool IsActive { get; set; }
        public int Point { get; set; }//số điểm cần để dùng mã giảm giá
    }
}