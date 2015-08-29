using System.Data.Entity.Migrations;

namespace HearthCap.Data.Migrations
{
    public partial class CascadeDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GameResults", "FK_dbo.GameResults_dbo.ArenaSessions_ArenaSessionId");
            AddForeignKey("dbo.GameResults", "ArenaSessionId", "dbo.ArenaSessions", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.GameResults", "FK_dbo.GameResults_dbo.ArenaSessions_ArenaSessionId");
            AddForeignKey("dbo.GameResults", "ArenaSessionId", "dbo.ArenaSessions", cascadeDelete: false);
        }
    }
}
