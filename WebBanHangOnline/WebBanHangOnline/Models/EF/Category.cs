using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Category")]
    public class Category : CommonAbstract
    {
        public Category() 
        {
            this.News = new HashSet<News>(); // quan hệ 1 - nhiều (1 danh mục có nhiều tin tức)
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SeoTitle { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoDescription { get; set; }
        public int Position { get; set; }

        public ICollection<News> News { get; set; }
        public ICollection<Posts> Posts { get; set; }
    }
}