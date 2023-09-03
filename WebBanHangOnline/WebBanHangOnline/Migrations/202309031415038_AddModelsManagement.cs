namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModelsManagement : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_ContactInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PhoneNumber = c.String(maxLength: 15),
                        Email = c.String(),
                        Address = c.String(),
                        City = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tb_Logo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogoImage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tb_Slider",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 150),
                        Description = c.String(maxLength: 500),
                        Image = c.String(maxLength: 500),
                        Link = c.String(maxLength: 500),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tb_SocialMediaProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogoSocialMedia = c.String(),
                        SocialMediaName = c.String(),
                        LinkSocialMedia = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tb_SocialMediaProfiles");
            DropTable("dbo.tb_Slider");
            DropTable("dbo.tb_Logo");
            DropTable("dbo.tb_ContactInfo");
        }
    }
}
