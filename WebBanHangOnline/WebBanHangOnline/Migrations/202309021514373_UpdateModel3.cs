namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModel3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_Order", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_Order", "Status", c => c.Boolean(nullable: false));
        }
    }
}
