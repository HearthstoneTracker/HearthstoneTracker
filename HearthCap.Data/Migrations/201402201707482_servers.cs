using System.Data.Entity.Migrations;

namespace HearthCap.Data.Migrations
{
    public partial class servers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ArenaSessions", "Server", c => c.String(maxLength: 4000));
            AddColumn("dbo.GameResults", "Server", c => c.String(maxLength: 4000));
        }

        public override void Down()
        {
            DropColumn("dbo.GameResults", "Server");
            DropColumn("dbo.ArenaSessions", "Server");
        }
    }
}
