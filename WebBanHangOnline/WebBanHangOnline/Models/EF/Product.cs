using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Product")]
    public class Product : CommonAbstract
    {

        public Product() 
        {
            this.ProductImage = new HashSet<ProductImage>(); // 1 sản phẩm có nhiều ảnh
            this.OrderDetails = new HashSet<OrderDetail>(); // 1 sản phẩm có nhiều chi tiết đơn hàng
            this.ReviewProducts = new HashSet<ReviewProduct>(); // 1 sản phẩm có nhiều review
            this.WishLists = new HashSet<WishList>(); // 1 sản phẩm có nhiều yêu thích
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng không để trống")]
        [StringLength(250)]
        public string Title { get; set; }
        [StringLength(250)]
        public string Alias { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        public int ProductCategoryID { get; set; }
        public string Description { get; set; }
        [AllowHtml]
        public string Detail { get; set; }
        [Required(ErrorMessage = "Vui lòng không để trống")]

        public string Image { get; set; }
        [Required(ErrorMessage = "Vui lòng không để trống")]
        public decimal OriginalPrice { get; set; }
        [Required(ErrorMessage = "Vui lòng không để trống")]
        public decimal Price { get; set; }
        public decimal? PriceSale { get; set; }
        public bool IsHome { get; set; }
        public bool IsSale { get; set; }
        public bool IsFeature { get; set; }
        public bool IsHot { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Vui lòng không để trống")]
        public int Quantity { get; set; }
        public int ViewCount { get; set; }
        public string SeoTitle { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoDescription { get; set; }
        public bool IsApprovedByAdmin { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<ProductImage> ProductImage { get; set; } // 1 sản phẩm có nhiều ảnh
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } // 1 sản phẩm có nhiều chi tiết đơn hàng
        public virtual ICollection<ReviewProduct> ReviewProducts { get; set; } // 1 sản phẩm có nhiều review
        public virtual ICollection<WishList> WishLists { get; set; } // 1 sản phẩm có nhiều yêu thích
        public int? StoreID { get; set; }
        public virtual Store Store { get; set; }
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

        public int BuyCount { get; set; }
    }
}