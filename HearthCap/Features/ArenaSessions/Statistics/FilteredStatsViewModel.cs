// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilteredStatsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The filtered stats view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.ArenaSessions.Statistics
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;

    /// <summary>
    /// The filtered stats view model.
    /// </summary>
    [Export(typeof(FilteredStatsViewModel))]
    public class FilteredStatsViewModel : Screen
    {
        /// <summary>
        /// The initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// The wins and losses.
        /// </summary>
        private readonly BindableCollection<StatModel> winsAndLosses = new BindableCollection<StatModel>();

        /// <summary>
        /// The heroes played.
        /// </summary>
        private readonly BindableCollection<StatModel> heroesPlayed = new BindableCollection<StatModel>();

        /// <summary>
        /// The heroes.
        /// </summary>
        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        /// <summary>
        /// The win ratios.
        /// </summary>
        private BindableCollection<StatModel> winRatios = new BindableCollection<StatModel>();

        /// <summary>
        /// The refreshing.
        /// </summary>
        private bool refreshing;

        /// <summary>
        /// The opponent heroes played.
        /// </summary>
        private readonly BindableCollection<StatModel> opponentHeroesPlayed = new BindableCollection<StatModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredStatsViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public FilteredStatsViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.AddDesignModeData();
            }
        }

        /// <summary>
        /// Gets or sets the global data.
        /// </summary>
        [Import]
        protected GlobalData GlobalData { get; set; }

        /// <summary>
        /// The add design mode data.
        /// </summary>
        private void AddDesignModeData()
        {
            this.WinsAndLosses.Add(new StatModel("Wins", 30));
            this.WinsAndLosses.Add(new StatModel("Losses", 70));

            this.WinRatios.Add(new StatModel("0-6", 10));
            this.WinRatios.Add(new StatModel("7+", 14));
            this.WinRatios.Add(new StatModel("12", 3));
        }

        /// <summary>
        /// Gets the wins and losses.
        /// </summary>
        public BindableCollection<StatModel> WinsAndLosses
        {
            get
            {
                return this.winsAndLosses;
            }
        }

        /// <summary>
        /// Gets the heroes played.
        /// </summary>
        public BindableCollection<StatModel> HeroesPlayed
        {
            get
            {
                return this.heroesPlayed;
            }
        }

        /// <summary>
        /// Gets the opponent heroes played.
        /// </summary>
        public BindableCollection<StatModel> OpponentHeroesPlayed
        {
            get
            {
                return this.opponentHeroesPlayed;
            }
        }

        /// <summary>
        /// Gets the win ratios.
        /// </summary>
        public BindableCollection<StatModel> WinRatios
        {
            get
            {
                return this.winRatios;
            }
        }

        /// <summary>
        /// The refresh from.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        public void RefreshFrom(Func<HearthStatsDbContext> context, Expression<Func<ArenaSession, bool>> filter)
        {
            if (this.refreshing) return;
            this.refreshing = true;

            if (!this.initialized)
            {
                this.InitializeData();
                this.initialized = true;
            }

            // Task.WaitAll(
            // Task.Run(() => CalculateWinsAndLosses(context(), filter)),
            // Task.Run(() => CalculateWinRatios(context(), filter)),
            // Task.Run(() => CalculateHeroesPlayed(context(), filter)),
            // Task.Run(() => CalculateOppHeroesPlayed(context(), filter))
            // ); 
            this.CalculateWinsAndLosses(context(), filter);
            this.CalculateWinRatios(context(), filter);
            this.CalculateWinRatios(context(), filter);
            this.CalculateHeroesPlayed(context(), filter);
            this.CalculateOppHeroesPlayed(context(), filter);
            
            this.refreshing = false;
        }

        /// <summary>
        /// The calculate win ratios.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        private void CalculateWinRatios(HearthStatsDbContext context, Expression<Func<ArenaSession, bool>> filter)
        {
            var arenas = context.ArenaSessions.Where(filter);

            // float total = results.Sum(a => a.Games.Count);
            float wins2 = arenas.Count(a => a.Wins <= 2);
            float wins3 = arenas.Count(a => a.Wins == 3);
            float wins4 = arenas.Count(a => a.Wins >= 4 && a.Wins <= 6);
            float wins7 = arenas.Count(a => a.Wins >= 7 && a.Wins <= 11);
            float wins12 = arenas.Count(a => a.Wins == 12);

            this.WinRatios.IsNotifying = false;
            this.WinRatios.Clear();
            this.WinRatios.Add(new StatModel("0-2", wins2));
            this.WinRatios.Add(new StatModel("3", wins3));
            this.WinRatios.Add(new StatModel("4-6", wins4));
            this.WinRatios.Add(new StatModel("7-11", wins7));
            this.WinRatios.Add(new StatModel("12", wins12));
            this.WinRatios.IsNotifying = true;
            this.WinRatios.Refresh();
        }

        /// <summary>
        /// The initialize data.
        /// </summary>
        private void InitializeData()
        {
            var data = this.GlobalData.Get();
            this.heroes.Clear();
            this.heroes.AddRange(data.Heroes);
        }

        /// <summary>
        /// The calculate opp heroes played.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        private void CalculateOppHeroesPlayed(HearthStatsDbContext context, Expression<Func<ArenaSession, bool>> filter)
        {
            var arenas = context.ArenaSessions.Where(filter);
            this.opponentHeroesPlayed.IsNotifying = false;
            this.opponentHeroesPlayed.Clear();
            int total = arenas.Count();
            if (total == 0)
            {
                return;
            }

            int gamesCount = context.Games.Count(x => arenas.Contains(x.ArenaSession));

            var oppheroestats = context.Games
                .Where(x => arenas.Contains(x.ArenaSession))
                .GroupBy(x => x.OpponentHero)
                .Where(x => x.Any()).Select(
                    x => new
                    {
                        x.Key, 
                        x.Key.ClassName, 
                        Count = x.Count()
                    }).ToList();
            foreach (var hero in oppheroestats)
            {
                if (hero.Count > 0)
                {
                    this.opponentHeroesPlayed.Add(new StatModel(string.Format("{0}: {1}", hero.ClassName, hero.Count), (float)hero.Count / gamesCount * 100, hero.Key.GetBrush()));
                }
            }

            this.opponentHeroesPlayed.IsNotifying = true;
            this.opponentHeroesPlayed.Refresh();
        }

        /// <summary>
        /// The calculate heroes played.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        private void CalculateHeroesPlayed(HearthStatsDbContext context, Expression<Func<ArenaSession, bool>> filter)
        {
            var arenas = context.ArenaSessions.Where(filter);
            this.heroesPlayed.IsNotifying = false;
            this.heroesPlayed.Clear();

            int total = arenas.Count();
            if (total == 0)
            {
                return;
            }

            var heroestats = arenas.GroupBy(x => x.Hero).Where(x => x.Any()).Select(
                x => new
                         {
                             x.Key, 
                             x.Key.ClassName, 
                             Count = x.Count()
                         }).ToList();

            foreach (var hero in heroestats)
            {
                if (hero.Count > 0)
                {
                    this.heroesPlayed.Add(new StatModel(string.Format("{0}: {1}", hero.ClassName, hero.Count), (float)hero.Count / total * 100, hero.Key.GetBrush()));
                }
            }

            this.heroesPlayed.IsNotifying = true;
            this.heroesPlayed.Refresh();
        }

        /// <summary>
        /// The calculate wins and losses.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        private void CalculateWinsAndLosses(HearthStatsDbContext context, Expression<Func<ArenaSession, bool>> filter)
        {
            var arenas = context.ArenaSessions.Where(filter);
            if (!arenas.Any())
            {
                this.WinsAndLosses.Add(new StatModel("Wins", 0));
                this.WinsAndLosses.Add(new StatModel("Losses", 0));
                return;                
            }

            float total = arenas.Sum(a => a.Wins + a.Losses);
            float wins = arenas.Sum(a => a.Wins);
            float losses = arenas.Sum(a => a.Losses);
            this.WinsAndLosses.Clear();
            if (total <= 0)
            {
                this.WinsAndLosses.Add(new StatModel("Wins", 0));
                this.WinsAndLosses.Add(new StatModel("Losses", 0));
                return;
            }

            this.WinsAndLosses.Add(new StatModel(string.Format("Wins: {0}", wins), wins / total * 100));
            this.WinsAndLosses.Add(new StatModel(string.Format("Losses: {0}", losses), losses / total * 100));
        }
    }
}