using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<ToxicWord> ToxicWords { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<ReviewProduct> ReviewProducts { get; set; }
        public DbSet<Matterial> Matterials { get; set; }
        public DbSet<OrderMatts> OrderMatts { get; set; }
        public DbSet<OrderDetailMatts> OrderDetailMatts { get; set; }
        public DbSet<SocialMediaPlugin> SocialMediaPlugins { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Logo> Logos { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SocialMediaProfiles> SocialMediaProfiles { get; set; }
        public DbSet<ThongKe> ThongKes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}