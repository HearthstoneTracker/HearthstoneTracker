// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201403120039256_deckimages.cs" company="">
//   
// </copyright>
// <summary>
//   The deckimages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The deckimages.
    /// </summary>
    public partial class deckimages : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.CreateTable(
                "dbo.DeckImages", 
                c => new
                    {
                        DeckId = c.Guid(nullable: false), 
                        Image = c.Binary(), 
                        Created = c.DateTime(nullable: false), 
                        Modified = c.DateTime(nullable: false), 
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"), 
                    })
                .PrimaryKey(t => t.DeckId)
                .ForeignKey("dbo.Decks", t => t.DeckId)
                .Index(t => t.DeckId);
            
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.DropForeignKey("dbo.DeckImages", "DeckId", "dbo.Decks");
            this.DropIndex("dbo.DeckImages", new[] { "DeckId" });
            this.DropTable("dbo.DeckImages");
        }
    }
}
