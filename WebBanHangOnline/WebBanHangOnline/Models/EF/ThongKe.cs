using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("ThongKes")]
    public class ThongKe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        public DateTime ThoiGian { get; set; }
        public long SoLuotTruyCap { get; set; }
    }
}