﻿using System;
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
        public int Quantity { get; set; }
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
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<OrderDetailMatts> OrderDetailMatts { get; set; }

    }
}