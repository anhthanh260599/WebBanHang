using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Storage")]
    public class Storage : CommonAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        public int StoreID { get; set; }
        public int MatterialID { get; set; }
        public int Quantity { get; set; }
        public virtual Store Store { get; set; }
        public virtual Matterial Matterial { get; set; }
    }
}