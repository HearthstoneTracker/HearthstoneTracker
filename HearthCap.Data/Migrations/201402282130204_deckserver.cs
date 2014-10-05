// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201402282130204_deckserver.cs" company="">
//   
// </copyright>
// <summary>
//   The deckserver.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The deckserver.
    /// </summary>
    public partial class deckserver : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.AddColumn("dbo.Decks", "Server", c => c.String(maxLength: 4000));
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.DropColumn("dbo.Decks", "Server");
        }
    }
}
