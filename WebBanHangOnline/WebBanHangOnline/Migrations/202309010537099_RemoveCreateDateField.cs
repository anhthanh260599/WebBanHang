namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCreateDateField : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "CreatedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "CreatedDate", c => c.DateTime(nullable: false));
        }
    }
}
