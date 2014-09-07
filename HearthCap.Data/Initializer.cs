namespace HearthCap.Data
{
    using System.Data.Entity;

    public sealed class DbInitializer
            : MigrateDatabaseToLatestVersion<HearthStatsDbContext, Migrations.Configuration>
    {
        public DbInitializer()
        {
        }
    }
}