// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201402280405179_decks.cs" company="">
//   
// </copyright>
// <summary>
//   The decks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The decks.
    /// </summary>
    public partial class decks : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.AddColumn("dbo.GameResults", "Deck_Id", c => c.Guid());
            this.AddColumn("dbo.Decks", "Created", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            this.AddColumn("dbo.Decks", "Modified", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            this.AddColumn("dbo.Decks", "Deleted", c => c.Boolean(nullable: false));
            this.CreateIndex("dbo.GameResults", "Deck_Id");
            this.AddForeignKey("dbo.GameResults", "Deck_Id", "dbo.Decks", "Id");
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.DropForeignKey("dbo.GameResults", "Deck_Id", "dbo.Decks");
            this.DropIndex("dbo.GameResults", new[] { "Deck_Id" });
            this.DropColumn("dbo.Decks", "Deleted");
            this.DropColumn("dbo.Decks", "Modified");
            this.DropColumn("dbo.Decks", "Created");
            this.DropColumn("dbo.GameResults", "Deck_Id");
        }
    }
}
