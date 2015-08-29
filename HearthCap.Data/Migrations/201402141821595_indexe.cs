using System.Data.Entity.Migrations;

namespace HearthCap.Data.Migrations
{
    public partial class indexe : DbMigration
    {
        public override void Up()
        {
            //CreateIndex("dbo.ArenaSessions", "Hero_Id");
            //CreateIndex("dbo.GameResults", "ArenaSessionId");
            //CreateIndex("dbo.GameResults", "Hero_Id");
            //CreateIndex("dbo.GameResults", "OpponentHero_Id");
            //CreateIndex("dbo.SettingsItems", "Settings_Id");
        }

        public override void Down()
        {
            //DropIndex("dbo.SettingsItems", new[] { "Settings_Id" });
            //DropIndex("dbo.GameResults", new[] { "OpponentHero_Id" });
            //DropIndex("dbo.GameResults", new[] { "Hero_Id" });
            //DropIndex("dbo.GameResults", new[] { "ArenaSessionId" });
            //DropIndex("dbo.ArenaSessions", new[] { "Hero_Id" });
        }
    }
}
