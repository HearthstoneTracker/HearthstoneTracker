// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201403091843393_created_timestamp.cs" company="">
//   
// </copyright>
// <summary>
//   The created_timestamp.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The created_timestamp.
    /// </summary>
    public partial class created_timestamp : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.AddColumn("dbo.ArenaSessions", "Created", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.ArenaSessions", "Modified", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.ArenaSessions", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            this.AddColumn("dbo.GameResults", "Created", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.GameResults", "Modified", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.GameResults", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            this.AddColumn("dbo.DeletedArenaSessions", "Created", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.DeletedArenaSessions", "Modified", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.DeletedArenaSessions", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            this.AddColumn("dbo.DeletedGameResults", "Created", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.DeletedGameResults", "Modified", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.DeletedGameResults", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));

            this.Sql("UPDATE GameResults SET Created = Started, Modified = Started");
            this.Sql("UPDATE ArenaSessions SET Created = StartDate, Modified = StartDate");
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.DropColumn("dbo.DeletedGameResults", "Timestamp");
            this.DropColumn("dbo.DeletedGameResults", "Modified");
            this.DropColumn("dbo.DeletedGameResults", "Created");
            this.DropColumn("dbo.DeletedArenaSessions", "Timestamp");
            this.DropColumn("dbo.DeletedArenaSessions", "Modified");
            this.DropColumn("dbo.DeletedArenaSessions", "Created");
            this.DropColumn("dbo.GameResults", "Timestamp");
            this.DropColumn("dbo.GameResults", "Modified");
            this.DropColumn("dbo.GameResults", "Created");
            this.DropColumn("dbo.ArenaSessions", "Timestamp");
            this.DropColumn("dbo.ArenaSessions", "Modified");
            this.DropColumn("dbo.ArenaSessions", "Created");
        }
    }
}
