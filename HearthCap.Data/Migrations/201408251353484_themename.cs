namespace HearthCap.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class themename : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ThemeConfigurations", "Theme", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ThemeConfigurations", "Theme", c => c.Int(nullable: false));
        }
    }
}
