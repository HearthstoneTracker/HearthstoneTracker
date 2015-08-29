using System.Data.Entity;
using HearthCap.Data.Migrations;

namespace HearthCap.Data
{
    public sealed class DbInitializer
        : MigrateDatabaseToLatestVersion<HearthStatsDbContext, Configuration>
    {
    }
}
