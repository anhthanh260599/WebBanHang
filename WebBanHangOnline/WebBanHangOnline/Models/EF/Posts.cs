﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Post")]
    public class Posts:CommonAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Title { get; set; }
        public string Alias { get; set; }
        public int CategoryID { get; set; }
        public string Description { get; set; }
        [AllowHtml]
        public string Detail { get; set; }
        public string Image { get; set; }
        public int ViewCount { get; set; }
        public string SeoTitle { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoDescription { get; set; }
        public bool IsActive { get; set; }

        public virtual Category Category { get; set; }
        public override void SetCreated()
        {
            this.CreateDate = DateTime.Now;
            this.ModifierDate = DateTime.Now;
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