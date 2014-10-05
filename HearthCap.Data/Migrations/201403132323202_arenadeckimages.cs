// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201403132323202_arenadeckimages.cs" company="">
//   
// </copyright>
// <summary>
//   The arenadeckimages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The arenadeckimages.
    /// </summary>
    public partial class arenadeckimages : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.CreateTable(
                "dbo.ArenaDeckImages", 
                c => new
                    {
                        Id = c.Guid(nullable: false), 
                        Image = c.Binary(), 
                        Created = c.DateTime(nullable: false), 
                        Modified = c.DateTime(nullable: false), 
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"), 
                    })
                .PrimaryKey(t => t.Id);
            
            this.AddColumn("dbo.ArenaSessions", "Image1_Id", c => c.Guid());
            this.AddColumn("dbo.ArenaSessions", "Image2_Id", c => c.Guid());
            this.CreateIndex("dbo.ArenaSessions", "Image1_Id");
            this.CreateIndex("dbo.ArenaSessions", "Image2_Id");
            this.AddForeignKey("dbo.ArenaSessions", "Image1_Id", "dbo.ArenaDeckImages", "Id");
            this.AddForeignKey("dbo.ArenaSessions", "Image2_Id", "dbo.ArenaDeckImages", "Id");
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.DropForeignKey("dbo.ArenaSessions", "Image2_Id", "dbo.ArenaDeckImages");
            this.DropForeignKey("dbo.ArenaSessions", "Image1_Id", "dbo.ArenaDeckImages");
            this.DropIndex("dbo.ArenaSessions", new[] { "Image2_Id" });
            this.DropIndex("dbo.ArenaSessions", new[] { "Image1_Id" });
            this.DropColumn("dbo.ArenaSessions", "Image2_Id");
            this.DropColumn("dbo.ArenaSessions", "Image1_Id");
            this.DropTable("dbo.ArenaDeckImages");
        }
    }
}
