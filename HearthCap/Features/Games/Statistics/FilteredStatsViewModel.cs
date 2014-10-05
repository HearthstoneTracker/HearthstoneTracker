// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilteredStatsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The filtered stats view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games.Statistics
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Features.Games.LatestGames;
    using HearthCap.Util;

    /// <summary>
    /// The filtered stats view model.
    /// </summary>
    [Export(typeof(FilteredStatsViewModel))]
    public class FilteredStatsViewModel : Screen
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The parent view model.
        /// </summary>
        private readonly LatestGamesViewModel parentViewModel;

        /// <summary>
        /// The initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// The wins and losses.
        /// </summary>
        private readonly BindableCollection<StatModel> winsAndLosses = new BindableCollection<StatModel>();

        /// <summary>
        /// The wins.
        /// </summary>
        private readonly BindableCollection<StatModel> wins = new BindableCollection<StatModel>();

        /// <summary>
        /// The losses.
        /// </summary>
        private readonly BindableCollection<StatModel> losses = new BindableCollection<StatModel>();

        /// <summary>
        /// The heroes played.
        /// </summary>
        private readonly BindableCollection<StatModel> heroesPlayed = new BindableCollection<StatModel>();

        /// <summary>
        /// The opponent heroes played.
        /// </summary>
        private readonly BindableCollection<StatModel> opponentHeroesPlayed = new BindableCollection<StatModel>();

        /// <summary>
        /// The heroes.
        /// </summary>
        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        /// <summary>
        /// The with coin.
        /// </summary>
        private readonly BindableCollection<StatModel> withCoin = new BindableCollection<StatModel>();

        /// <summary>
        /// The without coin.
        /// </summary>
        private readonly BindableCollection<StatModel> withoutCoin = new BindableCollection<StatModel>();

        /// <summary>
        /// The refreshing.
        /// </summary>
        private bool refreshing;

        /// <summary>
        /// The today ratio win.
        /// </summary>
        private decimal todayRatioWin;

        /// <summary>
        /// The today ratio loss.
        /// </summary>
        private decimal todayRatioLoss;

        /// <summary>
        /// The this week ratio win.
        /// </summary>
        private decimal thisWeekRatioWin;

        /// <summary>
        /// The this week ratio loss.
        /// </summary>
        private decimal thisWeekRatioLoss;

        /// <summary>
        /// The this month ratio win.
        /// </summary>
        private decimal thisMonthRatioWin;

        /// <summary>
        /// The this month ratio loss.
        /// </summary>
        private decimal thisMonthRatioLoss;

        /// <summary>
        /// The last 7 days ratio win.
        /// </summary>
        private decimal last7DaysRatioWin;

        /// <summary>
        /// The last 7 days ratio loss.
        /// </summary>
        private decimal last7DaysRatioLoss;

        /// <summary>
        /// The last 30 days ratio win.
        /// </summary>
        private decimal last30DaysRatioWin;

        /// <summary>
        /// The last 30 days ratio loss.
        /// </summary>
        private decimal last30DaysRatioLoss;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredStatsViewModel"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        [ImportingConstructor]
        public FilteredStatsViewModel(Func<HearthStatsDbContext> dbContext)
        {
            this.dbContext = dbContext;
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
        /// Gets or sets the latest games view model.
        /// </summary>
        [Import]
        protected LatestGamesViewModel LatestGamesViewModel { get; set; }

        /// <summary>
        /// The add design mode data.
        /// </summary>
        private void AddDesignModeData()
        {
            this.WinsAndLosses.Add(new StatModel("Wins", 30));
            this.WinsAndLosses.Add(new StatModel("Losses", 70));
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
        /// Gets the wins.
        /// </summary>
        public BindableCollection<StatModel> Wins
        {
            get
            {
                return this.wins;
            }
        }

        /// <summary>
        /// Gets the losses.
        /// </summary>
        public BindableCollection<StatModel> Losses
        {
            get
            {
                return this.losses;
            }
        }

        /// <summary>
        /// Gets the with coin.
        /// </summary>
        public BindableCollection<StatModel> WithCoin
        {
            get
            {
                return this.withCoin;
            }
        }

        /// <summary>
        /// Gets the without coin.
        /// </summary>
        public BindableCollection<StatModel> WithoutCoin
        {
            get
            {
                return this.withoutCoin;
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
        /// Gets or sets the today ratio win.
        /// </summary>
        public decimal TodayRatioWin
        {
            get
            {
                return this.todayRatioWin;
            }

            set
            {
                if (value == this.todayRatioWin)
                {
                    return;
                }

                this.todayRatioWin = value;
                this.NotifyOfPropertyChange(() => this.TodayRatio);
                this.NotifyOfPropertyChange(() => this.TodayRatioWin);
            }
        }

        /// <summary>
        /// Gets or sets the today ratio loss.
        /// </summary>
        public decimal TodayRatioLoss
        {
            get
            {
                return this.todayRatioLoss;
            }

            set
            {
                if (value == this.todayRatioLoss)
                {
                    return;
                }

                this.todayRatioLoss = value;
                this.NotifyOfPropertyChange(() => this.TodayRatio);
                this.NotifyOfPropertyChange(() => this.TodayRatioLoss);
            }
        }

        /// <summary>
        /// Gets the today ratio.
        /// </summary>
        public decimal TodayRatio
        {
            get
            {
                if (this.TodayRatioLoss == 0) return 0;
                return Math.Round(this.TodayRatioWin / this.TodayRatioLoss, 3);
            }
        }

        /// <summary>
        /// Gets or sets the this week ratio win.
        /// </summary>
        public decimal ThisWeekRatioWin
        {
            get
            {
                return this.thisWeekRatioWin;
            }

            set
            {
                if (value == this.thisWeekRatioWin)
                {
                    return;
                }

                this.thisWeekRatioWin = value;
                this.NotifyOfPropertyChange(() => this.ThisWeekRatioWin);
                this.NotifyOfPropertyChange(() => this.ThisWeekRatio);
            }
        }

        /// <summary>
        /// Gets or sets the this week ratio loss.
        /// </summary>
        public decimal ThisWeekRatioLoss
        {
            get
            {
                return this.thisWeekRatioLoss;
            }

            set
            {
                if (value == this.thisWeekRatioLoss)
                {
                    return;
                }

                this.thisWeekRatioLoss = value;
                this.NotifyOfPropertyChange(() => this.ThisWeekRatioLoss);
                this.NotifyOfPropertyChange(() => this.ThisWeekRatio);
            }
        }

        /// <summary>
        /// Gets the this week ratio.
        /// </summary>
        public decimal ThisWeekRatio
        {
            get
            {
                if (this.ThisWeekRatioLoss == 0) return 0;
                return Math.Round(this.ThisWeekRatioWin / this.ThisWeekRatioLoss, 3);
            }
        }

        /// <summary>
        /// Gets or sets the this month ratio win.
        /// </summary>
        public decimal ThisMonthRatioWin
        {
            get
            {
                return this.thisMonthRatioWin;
            }

            set
            {
                if (value == this.thisMonthRatioWin)
                {
                    return;
                }

                this.thisMonthRatioWin = value;
                this.NotifyOfPropertyChange(() => this.ThisMonthRatioWin);
                this.NotifyOfPropertyChange(() => this.ThisMonthRatio);
            }
        }

        /// <summary>
        /// Gets or sets the this month ratio loss.
        /// </summary>
        public decimal ThisMonthRatioLoss
        {
            get
            {
                return this.thisMonthRatioLoss;
            }

            set
            {
                if (value == this.thisMonthRatioLoss)
                {
                    return;
                }

                this.thisMonthRatioLoss = value;
                this.NotifyOfPropertyChange(() => this.ThisMonthRatioLoss);
                this.NotifyOfPropertyChange(() => this.ThisMonthRatio);
            }
        }

        /// <summary>
        /// Gets the this month ratio.
        /// </summary>
        public decimal ThisMonthRatio
        {
            get
            {
                if (this.ThisMonthRatioLoss == 0) return 0;
                return Math.Round(this.ThisMonthRatioWin / this.ThisMonthRatioLoss, 3);
            }
        }

        /// <summary>
        /// Gets or sets the last 7 days ratio win.
        /// </summary>
        public decimal Last7DaysRatioWin
        {
            get
            {
                return this.last7DaysRatioWin;
            }

            set
            {
                if (value == this.last7DaysRatioWin)
                {
                    return;
                }

                this.last7DaysRatioWin = value;
                this.NotifyOfPropertyChange(() => this.Last7DaysRatioWin);
                this.NotifyOfPropertyChange(() => this.Last7DaysRatio);
            }
        }

        /// <summary>
        /// Gets or sets the last 7 days ratio loss.
        /// </summary>
        public decimal Last7DaysRatioLoss
        {
            get
            {
                return this.last7DaysRatioLoss;
            }

            set
            {
                if (value == this.last7DaysRatioLoss)
                {
                    return;
                }

                this.last7DaysRatioLoss = value;
                this.NotifyOfPropertyChange(() => this.Last7DaysRatioLoss);
                this.NotifyOfPropertyChange(() => this.Last7DaysRatio);
            }
        }

        /// <summary>
        /// Gets the last 7 days ratio.
        /// </summary>
        public decimal Last7DaysRatio
        {
            get
            {
                if (this.Last7DaysRatioLoss == 0) return 0;
                return Math.Round(this.Last7DaysRatioWin / this.Last7DaysRatioLoss, 3);
            }
        }

        /// <summary>
        /// Gets or sets the last 30 days ratio win.
        /// </summary>
        public decimal Last30DaysRatioWin
        {
            get
            {
                return this.last30DaysRatioWin;
            }

            set
            {
                if (value == this.last30DaysRatioWin)
                {
                    return;
                }

                this.last30DaysRatioWin = value;
                this.NotifyOfPropertyChange(() => this.Last30DaysRatioWin);
                this.NotifyOfPropertyChange(() => this.Last30DaysRatio);
            }
        }

        /// <summary>
        /// Gets or sets the last 30 days ratio loss.
        /// </summary>
        public decimal Last30DaysRatioLoss
        {
            get
            {
                return this.last30DaysRatioLoss;
            }

            set
            {
                if (value == this.last30DaysRatioLoss)
                {
                    return;
                }

                this.last30DaysRatioLoss = value;
                this.NotifyOfPropertyChange(() => this.Last30DaysRatioLoss);
                this.NotifyOfPropertyChange(() => this.Last30DaysRatio);
            }
        }

        /// <summary>
        /// Gets the last 30 days ratio.
        /// </summary>
        public decimal Last30DaysRatio
        {
            get
            {
                if (this.Last30DaysRatioLoss == 0) return 0;
                return Math.Round(this.Last30DaysRatioWin / this.Last30DaysRatioLoss, 3);
            }
        }

        /// <summary>
        /// The refresh from.
        /// </summary>
        /// <param name="contextFactory">
        /// The context factory.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        public void RefreshFrom(Func<HearthStatsDbContext> contextFactory, Expression<Func<GameResult, bool>> filter)
        {
            if (this.refreshing) return;
            this.refreshing = true;
            if (!this.initialized)
            {
                this.InitializeData();
                this.initialized = true;
            }
            
            // Task.WaitAll(
            // Task.Run(() => CalculateWinsAndLosses(contextFactory(), filter)),
            // Task.Run(() => CalculateHeroesPlayed(contextFactory(), filter)),
            // Task.Run(() => CalculateRatios(contextFactory()))
            // );
            this.CalculateWinsAndLosses(contextFactory(), filter);
            this.CalculateHeroesPlayed(contextFactory(), filter);
            this.CalculateRatios(contextFactory());

            this.refreshing = false;
        }

        /// <summary>
        /// The calculate ratios.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private void CalculateRatios(HearthStatsDbContext context)
        {
            var now = DateTime.Now;
            var start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            decimal numGames;
            decimal numWon;
            decimal numLoss;
            this.GetStatsSince(context, start, out numGames, out numWon, out numLoss);

            if (numGames > 0)
            {
                this.TodayRatioWin = Math.Round((numWon / numGames) * 100, 0);
                this.TodayRatioLoss = Math.Round((numLoss / numGames) * 100, 0);
            }
            else
            {
                this.TodayRatioWin = 0;
                this.TodayRatioLoss = 0;
            }

            start = now.StartOfWeek(DayOfWeek.Monday);
            this.GetStatsSince(context, start, out numGames, out numWon, out numLoss);
            if (numGames > 0)
            {
                this.ThisWeekRatioWin = Math.Round((numWon / numGames) * 100, 0);
                this.ThisWeekRatioLoss = Math.Round((numLoss / numGames) * 100, 0);
            }
            else
            {
                this.ThisWeekRatioWin = 0;
                this.ThisWeekRatioLoss = 0;
            }

            start = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
            this.GetStatsSince(context, start, out numGames, out numWon, out numLoss);
            if (numGames > 0)
            {
                this.ThisMonthRatioWin = Math.Round((numWon / numGames) * 100, 0);
                this.ThisMonthRatioLoss = Math.Round((numLoss / numGames) * 100, 0);
            }
            else
            {
                this.ThisMonthRatioWin = 0;
                this.ThisMonthRatioLoss = 0;
            }

            start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            start = start.AddDays(-7);
            this.GetStatsSince(context, start, out numGames, out numWon, out numLoss);
            if (numGames > 0)
            {
                this.Last7DaysRatioWin = Math.Round((numWon / numGames) * 100, 0);
                this.Last7DaysRatioLoss = Math.Round((numLoss / numGames) * 100, 0);
            }
            else
            {
                this.Last7DaysRatioWin = 0;
                this.Last7DaysRatioLoss = 0;
            }

            start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            start = start.AddDays(-30);
            this.GetStatsSince(context, start, out numGames, out numWon, out numLoss);
            if (numGames > 0)
            {
                this.Last30DaysRatioWin = Math.Round((numWon / numGames) * 100, 0);
                this.Last30DaysRatioLoss = Math.Round((numLoss / numGames) * 100, 0);
            }
            else
            {
                this.Last30DaysRatioWin = 0;
                this.Last30DaysRatioLoss = 0;
            }

        }

        /// <summary>
        /// The get stats since.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="numGames">
        /// The num games.
        /// </param>
        /// <param name="numWon">
        /// The num won.
        /// </param>
        /// <param name="numLoss">
        /// The num loss.
        /// </param>
        private void GetStatsSince(HearthStatsDbContext context, DateTime start, out decimal numGames, out decimal numWon, out decimal numLoss)
        {
            GameMode gameMode;
            bool filterGameMode = Enum.TryParse(this.LatestGamesViewModel.FilterGameMode, out gameMode);
            Guid heroId = Guid.Empty;
            bool filterHero = this.LatestGamesViewModel.FilterHero != null && !string.IsNullOrEmpty(this.LatestGamesViewModel.FilterHero.Key);
            if (filterHero) heroId = this.LatestGamesViewModel.FilterHero.Id;
            Guid oppHeroId = Guid.Empty;
            bool filterOppHero = this.LatestGamesViewModel.FilterOpponentHero != null && !string.IsNullOrEmpty(this.LatestGamesViewModel.FilterOpponentHero.Key);
            if (filterOppHero) oppHeroId = this.LatestGamesViewModel.FilterOpponentHero.Id;
            Guid deckId = Guid.Empty;
            bool filterDeck = this.LatestGamesViewModel.FilterDeck != null && !string.IsNullOrEmpty(this.LatestGamesViewModel.FilterDeck.Key);
            if (filterDeck) deckId = this.LatestGamesViewModel.FilterDeck.Id;

            numGames = context.Games.Count(
                x => x.Started > start
                     && (!filterGameMode || (filterGameMode && x.GameMode == gameMode))
                     && (!filterHero || (filterHero && x.Hero.Id == heroId))
                     && (!filterOppHero || (filterOppHero && x.OpponentHero.Id == oppHeroId))
                     && (!filterDeck || (filterDeck && x.Deck.Id == deckId)));
            numWon = context.Games.Count(
                x => x.Started > start && x.Victory
                     && (!filterGameMode || (filterGameMode && x.GameMode == gameMode))
                     && (!filterHero || (filterHero && x.Hero.Id == heroId))
                     && (!filterOppHero || (filterOppHero && x.OpponentHero.Id == oppHeroId))
                     && (!filterDeck || (filterDeck && x.Deck.Id == deckId)));
            numLoss = context.Games.Count(
                x => x.Started > start && !x.Victory
                     && (!filterGameMode || (filterGameMode && x.GameMode == gameMode))
                     && (!filterHero || (filterHero && x.Hero.Id == heroId))
                     && (!filterOppHero || (filterOppHero && x.OpponentHero.Id == oppHeroId))
                     && (!filterDeck || (filterDeck && x.Deck.Id == deckId)));
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
        /// The calculate heroes played.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        private void CalculateHeroesPlayed(HearthStatsDbContext context, Expression<Func<GameResult, bool>> filter)
        {
            var games = context.Games;

            this.heroesPlayed.Clear();
            this.opponentHeroesPlayed.Clear();
            int total = games.Where(filter).Count();
            if (total == 0) return;
            var heroestats = games
                .Where(filter)
                .GroupBy(x => x.Hero)
                .Where(x => x.Any())
                .Select(
                    x => new
                             {
                                 x.Key, 
                                 x.Key.ClassName, 
                                 Count = x.Count()
                             }).ToList();

            var oppheroestats = games
                .Where(filter)
                .GroupBy(x => x.OpponentHero)
                .Where(x => x.Any())
                .Select(
                    x => new
                             {
                                 x.Key, 
                                 x.Key.ClassName, 
                                 Count = x.Count()
                             }).ToList();

            this.heroesPlayed.IsNotifying = false;
            foreach (var hero in heroestats)
            {
                // float count = games.Where(filter).Count(x => x.Hero != null && x.Hero.Key == hero.Key);
                if (hero.Count > 0)
                {
                    this.heroesPlayed.Add(new StatModel(string.Format("{0}: {1}", hero.ClassName, hero.Count), (float)hero.Count / total * 100, hero.Key.GetBrush()));
                }
            }

            this.heroesPlayed.IsNotifying = true;
            this.heroesPlayed.Refresh();

            this.opponentHeroesPlayed.IsNotifying = false;
            foreach (var hero in oppheroestats)
            {
                // count = games.Where(filter).Count(x => x.OpponentHero != null && x.OpponentHero.Key == hero.Key);
                if (hero.Count > 0)
                {
                    this.opponentHeroesPlayed.Add(new StatModel(string.Format("{0}: {1}", hero.ClassName, hero.Count), (float)hero.Count / total * 100, hero.Key.GetBrush()));
                }
            }

            this.opponentHeroesPlayed.IsNotifying = true;
            this.opponentHeroesPlayed.Refresh();
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
        private void CalculateWinsAndLosses(HearthStatsDbContext context, Expression<Func<GameResult, bool>> filter)
        {
            var games = context.Games;
            float total = games.Where(filter).Count();
            float winsC = games.Where(filter).Count(x => x.Victory && !x.GoFirst);
            float lossesC = games.Where(filter).Count(x => !x.Victory && !x.GoFirst);
            float winsNC = games.Where(filter).Count(x => x.Victory && x.GoFirst);
            float lossesNC = games.Where(filter).Count(x => !x.Victory && x.GoFirst);
            float wins = winsC + winsNC;
            float losses = lossesC + lossesNC;

            this.WinsAndLosses.Clear();
            this.Wins.Clear();
            this.Losses.Clear();
            this.WithCoin.Clear();
            this.WithoutCoin.Clear();
            if (total <= 0)
            {
                this.WinsAndLosses.Add(new StatModel("Wins", 0));
                this.WinsAndLosses.Add(new StatModel("Losses", 0));
                this.Wins.Add(new StatModel("Coin", 0));
                this.Wins.Add(new StatModel("No coin", 0));
                this.Losses.Add(new StatModel("Coin", 0));
                this.Losses.Add(new StatModel("No coin", 0));
                this.WithCoin.Add(new StatModel("Losses", 0));
                this.WithCoin.Add(new StatModel("Losses", 0));
                this.WithoutCoin.Add(new StatModel("Losses", 0));
                this.WithoutCoin.Add(new StatModel("Losses", 0));

                return;
            }

            this.WinsAndLosses.Add(new StatModel(string.Format("Wins: {0}", wins), wins / total * 100));
            this.WinsAndLosses.Add(new StatModel(string.Format("Losses: {0}", losses), losses / total * 100));

            this.Wins.Add(new StatModel(string.Format("Coin: {0}", winsC), winsC / wins * 100));
            this.Wins.Add(new StatModel(string.Format("No coin: {0}", winsNC), winsNC / wins * 100));

            this.Losses.Add(new StatModel(string.Format("Coin: {0}", lossesC), lossesC / losses * 100));
            this.Losses.Add(new StatModel(string.Format("No coin: {0}", lossesNC), lossesNC / losses * 100));

            this.WithCoin.Add(new StatModel(string.Format("Wins: {0}", winsC), winsC / (winsC + lossesC) * 100));
            this.WithCoin.Add(new StatModel(string.Format("Losses: {0}", lossesC), lossesC / (winsC + lossesC) * 100));

            this.WithoutCoin.Add(new StatModel(string.Format("Wins: {0}", winsNC), winsNC / (winsNC + lossesNC) * 100));
            this.WithoutCoin.Add(new StatModel(string.Format("Losses: {0}", lossesNC), lossesNC / (winsNC + lossesNC) * 100));
        }
    }
}