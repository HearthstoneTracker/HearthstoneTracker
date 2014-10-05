// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201408251353484_themename.cs" company="">
//   
// </copyright>
// <summary>
//   The themename.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The themename.
    /// </summary>
    public partial class themename : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.AlterColumn("dbo.ThemeConfigurations", "Theme", c => c.String(maxLength: 4000));
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.AlterColumn("dbo.ThemeConfigurations", "Theme", c => c.Int(nullable: false));
        }
    }
}
