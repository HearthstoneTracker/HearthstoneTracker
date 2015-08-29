using System.Data.Entity.Migrations;

namespace HearthCap.Data.Migrations
{
    public partial class deckimages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeckImages",
                c => new
                    {
                        DeckId = c.Guid(nullable: false),
                        Image = c.Binary(),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion")
                    })
                .PrimaryKey(t => t.DeckId)
                .ForeignKey("dbo.Decks", t => t.DeckId)
                .Index(t => t.DeckId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.DeckImages", "DeckId", "dbo.Decks");
            DropIndex("dbo.DeckImages", new[] { "DeckId" });
            DropTable("dbo.DeckImages");
        }
    }
}
