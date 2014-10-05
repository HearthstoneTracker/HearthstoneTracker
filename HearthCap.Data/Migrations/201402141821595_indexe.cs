// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201402141821595_indexe.cs" company="">
//   
// </copyright>
// <summary>
//   The indexe.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The indexe.
    /// </summary>
    public partial class indexe : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            // CreateIndex("dbo.ArenaSessions", "Hero_Id");
            // CreateIndex("dbo.GameResults", "ArenaSessionId");
            // CreateIndex("dbo.GameResults", "Hero_Id");
            // CreateIndex("dbo.GameResults", "OpponentHero_Id");
            // CreateIndex("dbo.SettingsItems", "Settings_Id");
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            // DropIndex("dbo.SettingsItems", new[] { "Settings_Id" });
            // DropIndex("dbo.GameResults", new[] { "OpponentHero_Id" });
            // DropIndex("dbo.GameResults", new[] { "Hero_Id" });
            // DropIndex("dbo.GameResults", new[] { "ArenaSessionId" });
            // DropIndex("dbo.ArenaSessions", new[] { "Hero_Id" });
        }
    }
}
