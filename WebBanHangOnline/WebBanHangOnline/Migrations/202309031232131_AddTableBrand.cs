namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddTableBrand : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_Brand",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Slider = c.String(maxLength: 500),
                    BrandLogo = c.String(maxLength: 500),
                    Footer = c.String(maxLength: 500),
                    ContactEmail = c.String(),
                    ContactPhone = c.String(),
                    ContactAddress = c.String(),
                    BrandName = c.String(maxLength: 500),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
        }
    }
}
