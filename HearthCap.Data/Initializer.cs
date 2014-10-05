// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Initializer.cs" company="">
//   
// </copyright>
// <summary>
//   The db initializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System.Data.Entity;

    using HearthCap.Data.Migrations;

    /// <summary>
    /// The db initializer.
    /// </summary>
    public sealed class DbInitializer
            : MigrateDatabaseToLatestVersion<HearthStatsDbContext, Configuration>
    {
    }
}