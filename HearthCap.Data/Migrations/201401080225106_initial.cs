namespace HearthCap.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ArenaSessions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        Wins = c.Int(nullable: false),
                        Losses = c.Int(nullable: false),
                        RewardGold = c.Int(nullable: false),
                        RewardDust = c.Int(nullable: false),
                        RewardPacks = c.Int(nullable: false),
                        RewardOther = c.String(maxLength: 4000),
                        Retired = c.Boolean(nullable: false),
                        Hero_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Heroes", t => t.Hero_Id)
                .Index(t => t.Hero_Id);
            
            CreateTable(
                "dbo.GameResults",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Victory = c.Boolean(nullable: false),
                        GoFirst = c.Boolean(nullable: false),
                        Started = c.DateTime(nullable: false),
                        Stopped = c.DateTime(nullable: false),
                        DeckKey = c.String(maxLength: 4000),
                        GameMode = c.Int(nullable: false),
                        Notes = c.String(maxLength: 4000),
                        ArenaSessionId = c.Guid(),
                        ArenaGameNo = c.Int(nullable: false),
                        Turns = c.Int(nullable: false),
                        Conceded = c.Boolean(nullable: false),
                        Hero_Id = c.Guid(),
                        OpponentHero_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Heroes", t => t.Hero_Id)
                .ForeignKey("dbo.Heroes", t => t.OpponentHero_Id)
                .ForeignKey("dbo.ArenaSessions", t => t.ArenaSessionId)
                .Index(t => t.Hero_Id)
                .Index(t => t.OpponentHero_Id)
                .Index(t => t.ArenaSessionId);
            
            CreateTable(
                "dbo.Heroes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(maxLength: 4000),
                        Description = c.String(maxLength: 4000),
                        Icon = c.String(maxLength: 4000),
                        ClassName = c.String(maxLength: 4000),
                        Key = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Decks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Key = c.String(maxLength: 4000),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SettingsItems",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 4000),
                        StringValue = c.String(maxLength: 4000),
                        IntValue = c.Int(nullable: false),
                        Settings_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Settings", t => t.Settings_Id)
                .Index(t => t.Settings_Id);
            
            CreateTable(
                "dbo.ThemeConfigurations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(maxLength: 4000),
                        Accent = c.String(maxLength: 4000),
                        Theme = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SettingsItems", "Settings_Id", "dbo.Settings");
            DropForeignKey("dbo.ArenaSessions", "Hero_Id", "dbo.Heroes");
            DropForeignKey("dbo.GameResults", "ArenaSessionId", "dbo.ArenaSessions");
            DropForeignKey("dbo.GameResults", "OpponentHero_Id", "dbo.Heroes");
            DropForeignKey("dbo.GameResults", "Hero_Id", "dbo.Heroes");
            DropIndex("dbo.SettingsItems", new[] { "Settings_Id" });
            DropIndex("dbo.ArenaSessions", new[] { "Hero_Id" });
            DropIndex("dbo.GameResults", new[] { "ArenaSessionId" });
            DropIndex("dbo.GameResults", new[] { "OpponentHero_Id" });
            DropIndex("dbo.GameResults", new[] { "Hero_Id" });
            DropTable("dbo.ThemeConfigurations");
            DropTable("dbo.SettingsItems");
            DropTable("dbo.Settings");
            DropTable("dbo.Decks");
            DropTable("dbo.Heroes");
            DropTable("dbo.GameResults");
            DropTable("dbo.ArenaSessions");
        }
    }
}
