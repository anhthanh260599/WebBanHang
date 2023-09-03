using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_SocialMediaProfiles")]
    public class SocialMediaProfiles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        public string LogoSocialMedia { get; set; }
        public string SocialMediaName { get; set; }
        public string LinkSocialMedia { get; set; }

    }
}