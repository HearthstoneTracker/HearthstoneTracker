// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201401241936465_TextFiles.cs" company="">
//   
// </copyright>
// <summary>
//   The text files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The text files.
    /// </summary>
    public partial class TextFiles : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.CreateTable(
                "dbo.TextFileTemplates", 
                c => new
                    {
                        Id = c.Guid(nullable: false), 
                        Filename = c.String(maxLength: 4000), 
                        Template = c.String(maxLength: 4000), 
                    })
                .PrimaryKey(t => t.Id);
            
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.DropTable("dbo.TextFileTemplates");
        }
    }
}
