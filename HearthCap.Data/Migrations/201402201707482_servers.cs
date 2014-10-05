// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201402201707482_servers.cs" company="">
//   
// </copyright>
// <summary>
//   The servers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The servers.
    /// </summary>
    public partial class servers : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.AddColumn("dbo.ArenaSessions", "Server", c => c.String(maxLength: 4000));
            this.AddColumn("dbo.GameResults", "Server", c => c.String(maxLength: 4000));
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.DropColumn("dbo.GameResults", "Server");
            this.DropColumn("dbo.ArenaSessions", "Server");
        }
    }
}
