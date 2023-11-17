using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Models
{
    public class MaterialQuantityViewModel
    {
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
        public int Quantity { get; set; }

        public MaterialQuantityViewModel(Matterial matterial, Storage storage)
        {
            Id = matterial.Id;
            Title = matterial.Title;
            Alias = matterial.Alias;
            MattsCode = matterial.MattsCode;
            Description = matterial.Description;
            Price = matterial.Price;
            Image = matterial.Image;
            IsActive = matterial.IsActive;
            OrderDetailMatts = matterial.OrderDetailMatts;
            Packing = matterial.Packing;
            Quantity = storage.Quantity;
        }
    }
}