using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Documents")]
    public class Documents : CommonAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public override void SetCreated()
        {
            this.CreateDate = DateTime.Now;
            this.ModifierDate = DateTime.Now;
            this.CreateBy = "Admin";
            this.ModifierBy = "Admin";
        }

        public override void SetModified()
        {
            this.ModifierDate = DateTime.Now;
            this.ModifierBy = "Admin";
        }
    }
}