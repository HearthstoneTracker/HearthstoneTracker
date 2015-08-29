using System;
using System.ComponentModel.Composition;
using System.Linq.Expressions;
using HearthCap.Data;
using HearthCap.Features.Games.Statistics;

namespace HearthCap.Features.Charts
{
    [Export(typeof(IChartTab))]
    public class CommonChartsViewModel : ChartTab
    {
        private readonly Func<HearthStatsDbContext> dbContext;

        [ImportingConstructor]
        public CommonChartsViewModel(Func<HearthStatsDbContext> dbContext)
        {
            DisplayName = "Overview";
            Order = 0;
            this.dbContext = dbContext;
        }

        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public FilteredStatsViewModel GamesStats { get; set; }

        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public ArenaSessions.Statistics.FilteredStatsViewModel ArenaStats { get; set; }

        public override void RefreshData(Expression<Func<GameResult, bool>> gameFilter, Expression<Func<ArenaSession, bool>> arenaFilter)
        {
            GamesStats.RefreshFrom(dbContext, gameFilter);
            ArenaStats.RefreshFrom(dbContext, arenaFilter);
        }
    }
}
