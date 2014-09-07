namespace HearthCap.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<HearthStatsDbContext>
    {
        public const int SeedVersion = 15;

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(HearthStatsDbContext context)
        {
            SeedHeroes(context);
            // SeedDecks(context);
            context.SaveChanges();

            // UpdateDecks(context);
            // context.SaveChanges();
        }

        //private void UpdateDecks(HearthStatsDbContext context)
        //{
        //    var decks = context.Decks.ToList();
        //    var games = context.Games.Where(x => !String.IsNullOrEmpty(x.DeckKey));
        //    foreach (var game in games)
        //    {
        //        game.DeckKey = null;
        //        if (game.GameMode == GameMode.Arena)
        //        {
        //            game.Deck = null;
        //        }
        //        else
        //        {
        //            game.Deck = decks.FirstOrDefault(x => x.Key == game.DeckKey && x.Server == game.Server);
        //        }
        //    }
        //}

        //private void SeedDecks(HearthStatsDbContext context)
        //{
        //    var decks = new List<Deck>();
        //    var servers = new[] { "EU", "NA", "CN", "Asia" };
        //    foreach (var server in servers)
        //    {
        //        for (int i = 1; i <= 9; i++)
        //        {
        //            decks.Add(new Deck()
        //            {
        //                Key = i.ToString(CultureInfo.InvariantCulture),
        //                Name = String.Format("Deck #{0}", i),
        //                Server = server
        //            });
        //        }                
        //    }

        //    context.Decks.AddOrUpdate(x => new { x.Key, x.Server }, decks.ToArray());
        //}

        private void SeedHeroes(HearthStatsDbContext context)
        {
            Hero[] heroes =
                {
                    new Hero("mage") { Name = "Jaina Proudmoore", ClassName = "Mage" },
                    new Hero("paladin") { Name = "Uther the Lightbringer", ClassName = "Paladin" },
                    new Hero("priest") { Name = "Anduin Wrynn", ClassName = "Priest" },
                    new Hero("druid") { Name = "Malfurion Stormrage", ClassName = "Druid" },
                    new Hero("hunter") { Name = "Rexxar", ClassName = "Hunter" },
                    new Hero("rogue") { Name = "Valeera Sanguinar", ClassName = "Rogue" },
                    new Hero("shaman") { Name = "Thrall", ClassName = "Shaman" },
                    new Hero("warlock") { Name = "Gul'dan", ClassName = "Warlock" },
                    new Hero("warrior") { Name = "Garrosh Hellscream", ClassName = "Warrior" },
                };

            context.Heroes.AddOrUpdate(x => x.Key, heroes);
        }
    }
}
