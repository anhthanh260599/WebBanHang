using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Recipe")]
    public class Recipe
    {
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tự động tăng
        public int ID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ProductID { get; set; }

        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng không để trống")]
        public string Title { get; set; }

        [Required]
        public DateTime DateCreate { get; set; }
        [Required]
        public DateTime DateEdit { get; set; }
        [AllowHtml]
        public string Note { get; set; }

        public virtual Product Product { get; set; }

    }
}