using System.Data.Entity.Migrations;

namespace HearthCap.Data.Migrations
{
    public partial class deckserver : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Decks", "Server", c => c.String(maxLength: 4000));
        }

        public override void Down()
        {
            DropColumn("dbo.Decks", "Server");
        }
    }
}
