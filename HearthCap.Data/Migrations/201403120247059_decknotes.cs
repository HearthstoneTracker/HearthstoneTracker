// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201403120247059_decknotes.cs" company="">
//   
// </copyright>
// <summary>
//   The decknotes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The decknotes.
    /// </summary>
    public partial class decknotes : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.AddColumn("dbo.Decks", "Notes", c => c.String(maxLength: 4000));
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.DropColumn("dbo.Decks", "Notes");
        }
    }
}
