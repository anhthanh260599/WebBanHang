namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WebBanHangOnline.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebBanHangOnline.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
                new ProductCategory() { Id = 2, Title = "Trà", Description = "Trà trái cây", Icon="", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new ProductCategory() { Id = 3, Title = "Sữa", Description = "Sữa tươi", Icon="", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now }
            );

            context.Products.AddOrUpdate(x => x.Id,
                new Product() { Id = 1, Title = "Cà phê đen không đường", ProductCode="", ProductCategoryID=1, Description = "Cà phê hạt", Detail="", Image="", Price=0, PriceSale=0, IsHome=true,IsSale=true, IsFeature = true, IsHot = true, Quantity=0, SeoTitle="", SeoKeywords="", SeoDescription="", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 2, Title = "Cà phê đen có đường", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 3, Title = "Cà phê đen đường đá", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 4, Title = "Cà phê đen nóng", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 5, Title = "Cà phê sữa", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now }

            );

                new ProductCategory() { Id = 2, Title = "Trà", Description = "Trà trái cây", Icon="", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new ProductCategory() { Id = 3, Title = "Sữa", Description = "Sữa tươi", Icon="", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now }
            );

            context.Products.AddOrUpdate(x => x.Id,
                new Product() { Id = 1, Title = "Cà phê đen không đường", ProductCode="", ProductCategoryID=1, Description = "Cà phê hạt", Detail="", Image="", Price=0, PriceSale=0, IsHome=true,IsSale=true, IsFeature = true, IsHot = true, Quantity=0, SeoTitle="", SeoKeywords="", SeoDescription="", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 2, Title = "Cà phê đen có đường", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 3, Title = "Cà phê đen đường đá", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 4, Title = "Cà phê đen nóng", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 5, Title = "Cà phê sữa", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now }

            );

                new ProductCategory() { Id = 2, Title = "Trà", Description = "Trà trái cây", Icon="", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new ProductCategory() { Id = 3, Title = "Sữa", Description = "Sữa tươi", Icon="", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now }
            );

            context.Products.AddOrUpdate(x => x.Id,
                new Product() { Id = 1, Title = "Cà phê đen không đường", ProductCode="", ProductCategoryID=1, Description = "Cà phê hạt", Detail="", Image="", Price=0, PriceSale=0, IsHome=true,IsSale=true, IsFeature = true, IsHot = true, Quantity=0, SeoTitle="", SeoKeywords="", SeoDescription="", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 2, Title = "Cà phê đen có đường", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 3, Title = "Cà phê đen đường đá", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 4, Title = "Cà phê đen nóng", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now },
                new Product() { Id = 5, Title = "Cà phê sữa", ProductCode = "", ProductCategoryID = 1, Description = "Cà phê hạt", Detail = "", Image = "", Price = 0, PriceSale = 0, IsHome = true, IsSale = true, IsFeature = true, IsHot = true, Quantity = 0, SeoTitle = "", SeoKeywords = "", SeoDescription = "", CreateBy = "Duy", CreateDate = DateTime.Parse("01-01-2023"), ModifierBy = "", ModifierDate = DateTime.Now }

            );

        }
    }
}
