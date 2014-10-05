// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HearthStatsDbContext.cs" company="">
//   
// </copyright>
// <summary>
//   The hearth stats db context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System.Data.Entity;

    /// <summary>
    /// The hearth stats db context.
    /// </summary>
    public class HearthStatsDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the heroes.
        /// </summary>
        public DbSet<Hero> Heroes { get; set; }

        /// <summary>
        /// Gets or sets the games.
        /// </summary>
        public DbSet<GameResult> Games { get; set; }

        /// <summary>
        /// Gets or sets the deleted games.
        /// </summary>
        public DbSet<DeletedGameResult> DeletedGames { get; set; }

        /// <summary>
        /// Gets or sets the theme configurations.
        /// </summary>
        public DbSet<ThemeConfiguration> ThemeConfigurations { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public DbSet<Settings> Settings { get; set; }

        /// <summary>
        /// Gets or sets the settings items.
        /// </summary>
        public DbSet<SettingsItem> SettingsItems { get; set; }

        /// <summary>
        /// Gets or sets the decks.
        /// </summary>
        public DbSet<Deck> Decks { get; set; }

        /// <summary>
        /// Gets or sets the arena deck images.
        /// </summary>
        public DbSet<ArenaDeckImage> ArenaDeckImages { get; set; }

        /// <summary>
        /// Gets or sets the arena sessions.
        /// </summary>
        public DbSet<ArenaSession> ArenaSessions { get; set; }

        /// <summary>
        /// Gets or sets the deleted arena sessions.
        /// </summary>
        public DbSet<DeletedArenaSession> DeletedArenaSessions { get; set; }

        /// <summary>
        /// Gets or sets the text file templates.
        /// </summary>
        public DbSet<TextFileTemplate> TextFileTemplates { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HearthStatsDbContext"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">
        /// The name or connection string.
        /// </param>
        public HearthStatsDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HearthStatsDbContext"/> class.
        /// </summary>
        public HearthStatsDbContext()
            : base("db")
        {

            this.Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        ///             before the model has been locked down and used to initialize the context.  The default
        ///             implementation of this method does nothing, but it can be overridden in a derived class
        ///             such that the model can be further configured before it is locked down.
        /// </summary>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        ///             is created.  The model for that context is then cached and is for all further instances of
        ///             the context in the app domain.  This caching can be disabled by setting the ModelCaching
        ///             property on the given ModelBuidler, but note that this can seriously degrade performance.
        ///             More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        ///             classes directly.
        /// </remarks>
        /// <param name="modelBuilder">
        /// The builder that defines the model for the context being created. 
        /// </param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(new DateTimeConvention());

            // base.OnModelCreating(modelBuilder);
        }
    }
}
