// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonChartsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The common charts view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Charts
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq.Expressions;

    using HearthCap.Data;
    using HearthCap.Features.Games.Statistics;

    /// <summary>
    /// The common charts view model.
    /// </summary>
    [Export(typeof(IChartTab))]
    public class CommonChartsViewModel : ChartTab
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonChartsViewModel"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        [ImportingConstructor]
        public CommonChartsViewModel(Func<HearthStatsDbContext> dbContext)
        {
            this.DisplayName = "Overview";
            this.Order = 0;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Gets or sets the games stats.
        /// </summary>
        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public FilteredStatsViewModel GamesStats { get; set; }

        /// <summary>
        /// Gets or sets the arena stats.
        /// </summary>
        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public ArenaSessions.Statistics.FilteredStatsViewModel ArenaStats { get; set; }

        /// <summary>
        /// The refresh data.
        /// </summary>
        /// <param name="gameFilter">
        /// The game filter.
        /// </param>
        /// <param name="arenaFilter">
        /// The arena filter.
        /// </param>
        public override void RefreshData(Expression<Func<GameResult, bool>> gameFilter, Expression<Func<ArenaSession, bool>> arenaFilter)
        {
            this.GamesStats.RefreshFrom(this.dbContext, gameFilter);
            this.ArenaStats.RefreshFrom(this.dbContext, arenaFilter);
        }
    }
}