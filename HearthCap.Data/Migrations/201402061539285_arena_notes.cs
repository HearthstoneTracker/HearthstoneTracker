// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201402061539285_arena_notes.cs" company="">
//   
// </copyright>
// <summary>
//   The arena_notes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The arena_notes.
    /// </summary>
    public partial class arena_notes : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.AddColumn("dbo.ArenaSessions", "Notes", c => c.String(maxLength: 4000));
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.DropColumn("dbo.ArenaSessions", "Notes");
        }
    }
}
