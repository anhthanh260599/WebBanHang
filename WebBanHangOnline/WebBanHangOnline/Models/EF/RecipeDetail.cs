using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_RecipeDetails")]
    public class RecipeDetail
    {
        [Key]
        [Column(Order = 1)]
        public int RecipeID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int RecipeProductID { get; set; }
        [Key]
        [Column(Order = 3)]
        public int MatterialID { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống")]
        public string Quantity { get; set; }

        public virtual Matterial Matterial { get; set; }
        public virtual Recipe Recipe { get; set; }

    }
}