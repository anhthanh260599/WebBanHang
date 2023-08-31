namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateThongKe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ThongKes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ThoiGian = c.DateTime(nullable: false),
                        SoLuotTruyCap = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.tb_News", "ViewCount", c => c.Int(nullable: false));
            AddColumn("dbo.tb_Post", "ViewCount", c => c.Int(nullable: false));
            AddColumn("dbo.tb_Product", "ViewCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_Product", "ViewCount");
            DropColumn("dbo.tb_Post", "ViewCount");
            DropColumn("dbo.tb_News", "ViewCount");
            DropTable("dbo.ThongKes");
        }
    }
}
