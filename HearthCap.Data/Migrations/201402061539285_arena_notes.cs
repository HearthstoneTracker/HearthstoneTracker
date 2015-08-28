namespace HearthCap.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class arena_notes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ArenaSessions", "Notes", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ArenaSessions", "Notes");
        }
    }
}
