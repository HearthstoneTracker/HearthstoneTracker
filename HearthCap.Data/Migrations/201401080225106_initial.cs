// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201401080225106_initial.cs" company="">
//   
// </copyright>
// <summary>
//   The initial.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The initial.
    /// </summary>
    public partial class initial : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            this.CreateTable(
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
            
            this.CreateTable(
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
            
            this.CreateTable(
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
            
            this.CreateTable(
                "dbo.Decks", 
                c => new
                    {
                        Id = c.Guid(nullable: false), 
                        Key = c.String(maxLength: 4000), 
                        Name = c.String(maxLength: 4000), 
                    })
                .PrimaryKey(t => t.Id);
            
            this.CreateTable(
                "dbo.Settings", 
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true), 
                        Key = c.String(maxLength: 4000), 
                    })
                .PrimaryKey(t => t.Id);
            
            this.CreateTable(
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
            
            this.CreateTable(
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

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            this.DropForeignKey("dbo.SettingsItems", "Settings_Id", "dbo.Settings");
            this.DropForeignKey("dbo.ArenaSessions", "Hero_Id", "dbo.Heroes");
            this.DropForeignKey("dbo.GameResults", "ArenaSessionId", "dbo.ArenaSessions");
            this.DropForeignKey("dbo.GameResults", "OpponentHero_Id", "dbo.Heroes");
            this.DropForeignKey("dbo.GameResults", "Hero_Id", "dbo.Heroes");
            this.DropIndex("dbo.SettingsItems", new[] { "Settings_Id" });
            this.DropIndex("dbo.ArenaSessions", new[] { "Hero_Id" });
            this.DropIndex("dbo.GameResults", new[] { "ArenaSessionId" });
            this.DropIndex("dbo.GameResults", new[] { "OpponentHero_Id" });
            this.DropIndex("dbo.GameResults", new[] { "Hero_Id" });
            this.DropTable("dbo.ThemeConfigurations");
            this.DropTable("dbo.SettingsItems");
            this.DropTable("dbo.Settings");
            this.DropTable("dbo.Decks");
            this.DropTable("dbo.Heroes");
            this.DropTable("dbo.GameResults");
            this.DropTable("dbo.ArenaSessions");
        }
    }
}
