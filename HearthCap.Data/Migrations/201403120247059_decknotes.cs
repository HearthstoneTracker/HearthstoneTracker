using System.Data.Entity.Migrations;

namespace HearthCap.Data.Migrations
{
    public partial class decknotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Decks", "Notes", c => c.String(maxLength: 4000));
        }

        public override void Down()
        {
            DropColumn("dbo.Decks", "Notes");
        }
    }
}
