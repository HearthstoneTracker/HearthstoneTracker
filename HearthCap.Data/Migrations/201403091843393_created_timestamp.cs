namespace HearthCap.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class created_timestamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ArenaSessions", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.ArenaSessions", "Modified", c => c.DateTime(nullable: false));
            AddColumn("dbo.ArenaSessions", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.GameResults", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.GameResults", "Modified", c => c.DateTime(nullable: false));
            AddColumn("dbo.GameResults", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.DeletedArenaSessions", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.DeletedArenaSessions", "Modified", c => c.DateTime(nullable: false));
            AddColumn("dbo.DeletedArenaSessions", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.DeletedGameResults", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.DeletedGameResults", "Modified", c => c.DateTime(nullable: false));
            AddColumn("dbo.DeletedGameResults", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));

            Sql("UPDATE GameResults SET Created = Started, Modified = Started");
            Sql("UPDATE ArenaSessions SET Created = StartDate, Modified = StartDate");
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeletedGameResults", "Timestamp");
            DropColumn("dbo.DeletedGameResults", "Modified");
            DropColumn("dbo.DeletedGameResults", "Created");
            DropColumn("dbo.DeletedArenaSessions", "Timestamp");
            DropColumn("dbo.DeletedArenaSessions", "Modified");
            DropColumn("dbo.DeletedArenaSessions", "Created");
            DropColumn("dbo.GameResults", "Timestamp");
            DropColumn("dbo.GameResults", "Modified");
            DropColumn("dbo.GameResults", "Created");
            DropColumn("dbo.ArenaSessions", "Timestamp");
            DropColumn("dbo.ArenaSessions", "Modified");
            DropColumn("dbo.ArenaSessions", "Created");
        }
    }
}
