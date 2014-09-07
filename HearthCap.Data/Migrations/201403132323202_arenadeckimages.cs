namespace HearthCap.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class arenadeckimages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
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
            
            AddColumn("dbo.ArenaSessions", "Image1_Id", c => c.Guid());
            AddColumn("dbo.ArenaSessions", "Image2_Id", c => c.Guid());
            CreateIndex("dbo.ArenaSessions", "Image1_Id");
            CreateIndex("dbo.ArenaSessions", "Image2_Id");
            AddForeignKey("dbo.ArenaSessions", "Image1_Id", "dbo.ArenaDeckImages", "Id");
            AddForeignKey("dbo.ArenaSessions", "Image2_Id", "dbo.ArenaDeckImages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ArenaSessions", "Image2_Id", "dbo.ArenaDeckImages");
            DropForeignKey("dbo.ArenaSessions", "Image1_Id", "dbo.ArenaDeckImages");
            DropIndex("dbo.ArenaSessions", new[] { "Image2_Id" });
            DropIndex("dbo.ArenaSessions", new[] { "Image1_Id" });
            DropColumn("dbo.ArenaSessions", "Image2_Id");
            DropColumn("dbo.ArenaSessions", "Image1_Id");
            DropTable("dbo.ArenaDeckImages");
        }
    }
}
