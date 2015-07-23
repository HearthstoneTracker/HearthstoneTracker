namespace HearthCap.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class decks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameResults", "Deck_Id", c => c.Guid());
            AddColumn("dbo.Decks", "Created", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("dbo.Decks", "Modified", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("dbo.Decks", "Deleted", c => c.Boolean(nullable: false));
            CreateIndex("dbo.GameResults", "Deck_Id");
            AddForeignKey("dbo.GameResults", "Deck_Id", "dbo.Decks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameResults", "Deck_Id", "dbo.Decks");
            DropIndex("dbo.GameResults", new[] { "Deck_Id" });
            DropColumn("dbo.Decks", "Deleted");
            DropColumn("dbo.Decks", "Modified");
            DropColumn("dbo.Decks", "Created");
            DropColumn("dbo.GameResults", "Deck_Id");
        }
    }
}
