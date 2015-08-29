using System.Data.Entity.Migrations;

namespace HearthCap.Data.Migrations
{
    public partial class TextFiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TextFileTemplates",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Filename = c.String(maxLength: 4000),
                        Template = c.String(maxLength: 4000)
                    })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.TextFileTemplates");
        }
    }
}
