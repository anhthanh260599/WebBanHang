using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Matterial")]
    public class Matterial : CommonAbstract
    {
        public Matterial()
        {
            this.OrderDetailMatts = new HashSet<OrderDetailMatts>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Title { get; set; }
        [StringLength(250)]
        public string Alias { get; set; }
        [StringLength(50)]
        public string MattsCode { get; set; }
        public string Description { get; set; }
        [AllowHtml]
        public string Detail { get; set; }
        public decimal Price { get; set; }

        public string Image { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<OrderDetailMatts> OrderDetailMatts { get; set; }
        public string Packing { get; set; }//Quy cách tính
        public virtual ICollection<Storage> Storages { get; set; }
        public override void SetCreated()
        {
            this.CreateDate = DateTime.Now;
            this.ModifierDate = DateTime.Now;
            this.CreateBy = "Admin";
        }
        public override void SetModified()
        {
            this.ModifierDate = DateTime.Now;
        }
        public override void SetCreatedBy()
        {

        }

        public override void SetModifiedBy()
        {

        }
    }
}