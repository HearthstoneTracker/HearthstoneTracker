namespace HearthCap.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeDelete : DbMigration
    {
        public override void Up()
        {
            this.DropForeignKey("dbo.GameResults", "FK_dbo.GameResults_dbo.ArenaSessions_ArenaSessionId");
            this.AddForeignKey("dbo.GameResults", "ArenaSessionId", "dbo.ArenaSessions", cascadeDelete: true);
        }
        
        public override void Down()
        {
            this.DropForeignKey("dbo.GameResults", "FK_dbo.GameResults_dbo.ArenaSessions_ArenaSessionId");
            this.AddForeignKey("dbo.GameResults", "ArenaSessionId", "dbo.ArenaSessions", cascadeDelete: false);
        }
    }
}
