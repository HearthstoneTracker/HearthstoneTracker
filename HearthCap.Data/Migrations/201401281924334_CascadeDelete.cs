// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201401281924334_CascadeDelete.cs" company="">
//   
// </copyright>
// <summary>
//   The cascade delete.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The cascade delete.
    /// </summary>
    public partial class CascadeDelete : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.DropForeignKey("dbo.GameResults", "FK_dbo.GameResults_dbo.ArenaSessions_ArenaSessionId");
            this.AddForeignKey("dbo.GameResults", "ArenaSessionId", "dbo.ArenaSessions", cascadeDelete: true);
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.DropForeignKey("dbo.GameResults", "FK_dbo.GameResults_dbo.ArenaSessions_ArenaSessionId");
            this.AddForeignKey("dbo.GameResults", "ArenaSessionId", "dbo.ArenaSessions", cascadeDelete: false);
        }
    }
}
