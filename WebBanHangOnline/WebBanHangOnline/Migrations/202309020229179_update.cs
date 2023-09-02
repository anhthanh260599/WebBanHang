﻿namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_Subscribe", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.tb_SystemSetting", "SettingValue", c => c.String(maxLength: 4000));
            AlterColumn("dbo.tb_SystemSetting", "SettingDescription", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_SystemSetting", "SettingDescription", c => c.String(maxLength: 500));
            AlterColumn("dbo.tb_SystemSetting", "SettingValue", c => c.String(maxLength: 500));
            AlterColumn("dbo.tb_Subscribe", "Email", c => c.String());
        }
    }
}
