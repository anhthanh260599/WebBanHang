using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Store")]
    public class Store
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Alias { get; set; }
        public string Image { get; set; }
        public string LinkMap { get; set; }

        [AllowHtml]
        public string EmbedMap { get; set; }
        public bool IsActive { get; set; }
        public string Address { get; set; }

        [Display(Name = "Tỉnh/Thành")]
        public string Province { get; set; }
        [Display(Name = "Quận/Huyện")]
        public string District { get; set; }
        [Display(Name = "Phường/Xã")]
        public string Ward { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}