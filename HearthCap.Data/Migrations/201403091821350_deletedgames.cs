namespace HearthCap.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletedgames : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeletedArenaSessions",
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
                        Notes = c.String(maxLength: 4000),
                        Retired = c.Boolean(nullable: false),
                        Server = c.String(maxLength: 4000),
                        DeletedDate = c.DateTime(nullable: false),
                        Hero_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Heroes", t => t.Hero_Id)
                .Index(t => t.Hero_Id);
            
            CreateTable(
                "dbo.DeletedGameResults",
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
                        Server = c.String(maxLength: 4000),
                        DeletedDate = c.DateTime(nullable: false),
                        Deck_Id = c.Guid(),
                        Hero_Id = c.Guid(),
                        OpponentHero_Id = c.Guid(),
                        DeletedArenaSession_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Decks", t => t.Deck_Id)
                .ForeignKey("dbo.Heroes", t => t.Hero_Id)
                .ForeignKey("dbo.Heroes", t => t.OpponentHero_Id)
                .ForeignKey("dbo.DeletedArenaSessions", t => t.DeletedArenaSession_Id)
                .Index(t => t.Deck_Id)
                .Index(t => t.Hero_Id)
                .Index(t => t.OpponentHero_Id)
                .Index(t => t.DeletedArenaSession_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DeletedArenaSessions", "Hero_Id", "dbo.Heroes");
            DropForeignKey("dbo.DeletedGameResults", "DeletedArenaSession_Id", "dbo.DeletedArenaSessions");
            DropForeignKey("dbo.DeletedGameResults", "OpponentHero_Id", "dbo.Heroes");
            DropForeignKey("dbo.DeletedGameResults", "Hero_Id", "dbo.Heroes");
            DropForeignKey("dbo.DeletedGameResults", "Deck_Id", "dbo.Decks");
            DropIndex("dbo.DeletedGameResults", new[] { "DeletedArenaSession_Id" });
            DropIndex("dbo.DeletedGameResults", new[] { "OpponentHero_Id" });
            DropIndex("dbo.DeletedGameResults", new[] { "Hero_Id" });
            DropIndex("dbo.DeletedGameResults", new[] { "Deck_Id" });
            DropIndex("dbo.DeletedArenaSessions", new[] { "Hero_Id" });
            DropTable("dbo.DeletedGameResults");
            DropTable("dbo.DeletedArenaSessions");
        }
    }
}
