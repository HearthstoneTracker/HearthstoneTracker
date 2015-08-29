using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;
using HearthCap.Data;
using HearthCap.Features.Core;
using HearthCap.Features.Games.LatestGames;
using HearthCap.Util;

namespace HearthCap.Features.Games.Statistics
{
    [Export(typeof(FilteredStatsViewModel))]
    public class FilteredStatsViewModel : Screen
    {
        private bool initialized;
        private readonly BindableCollection<StatModel> winsAndLosses = new BindableCollection<StatModel>();
        private readonly BindableCollection<StatModel> wins = new BindableCollection<StatModel>();
        private readonly BindableCollection<StatModel> losses = new BindableCollection<StatModel>();

        private readonly BindableCollection<StatModel> heroesPlayed = new BindableCollection<StatModel>();
        private readonly BindableCollection<StatModel> opponentHeroesPlayed = new BindableCollection<StatModel>();

        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        private readonly BindableCollection<StatModel> withCoin = new BindableCollection<StatModel>();
        private readonly BindableCollection<StatModel> withoutCoin = new BindableCollection<StatModel>();

        private bool refreshing;

        private decimal todayRatioWin;

        private decimal todayRatioLoss;

        private decimal thisWeekRatioWin;

        private decimal thisWeekRatioLoss;

        private decimal thisMonthRatioWin;

        private decimal thisMonthRatioLoss;

        private decimal last7DaysRatioWin;

        private decimal last7DaysRatioLoss;

        private decimal last30DaysRatioWin;

        private decimal last30DaysRatioLoss;

        [ImportingConstructor]
        public FilteredStatsViewModel()
        {
            if (Execute.InDesignMode)
            {
                AddDesignModeData();
            }
        }

        [Import]
        protected GlobalData GlobalData { get; set; }

        [Import]
        protected LatestGamesViewModel LatestGamesViewModel { get; set; }

        private void AddDesignModeData()
        {
            WinsAndLosses.Add(new StatModel("Wins", 30));
            WinsAndLosses.Add(new StatModel("Losses", 70));
        }

        public BindableCollection<StatModel> WinsAndLosses
        {
            get { return winsAndLosses; }
        }

        public BindableCollection<StatModel> Wins
        {
            get { return wins; }
        }

        public BindableCollection<StatModel> Losses
        {
            get { return losses; }
        }

        public BindableCollection<StatModel> WithCoin
        {
            get { return withCoin; }
        }

        public BindableCollection<StatModel> WithoutCoin
        {
            get { return withoutCoin; }
        }

        public BindableCollection<StatModel> HeroesPlayed
        {
            get { return heroesPlayed; }
        }

        public BindableCollection<StatModel> OpponentHeroesPlayed
        {
            get { return opponentHeroesPlayed; }
        }

        public decimal TodayRatioWin
        {
            get { return todayRatioWin; }
            set
            {
                if (value == todayRatioWin)
                {
                    return;
                }
                todayRatioWin = value;
                NotifyOfPropertyChange(() => TodayRatio);
                NotifyOfPropertyChange(() => TodayRatioWin);
            }
        }

        public decimal TodayRatioLoss
        {
            get { return todayRatioLoss; }
            set
            {
                if (value == todayRatioLoss)
                {
                    return;
                }
                todayRatioLoss = value;
                NotifyOfPropertyChange(() => TodayRatio);
                NotifyOfPropertyChange(() => TodayRatioLoss);
            }
        }

        public decimal TodayRatio
        {
            get
            {
                if (TodayRatioLoss == 0)
                {
                    return 0;
                }
                return Math.Round(TodayRatioWin / TodayRatioLoss, 3);
            }
        }

        public decimal ThisWeekRatioWin
        {
            get { return thisWeekRatioWin; }
            set
            {
                if (value == thisWeekRatioWin)
                {
                    return;
                }
                thisWeekRatioWin = value;
                NotifyOfPropertyChange(() => ThisWeekRatioWin);
                NotifyOfPropertyChange(() => ThisWeekRatio);
            }
        }

        public decimal ThisWeekRatioLoss
        {
            get { return thisWeekRatioLoss; }
            set
            {
                if (value == thisWeekRatioLoss)
                {
                    return;
                }
                thisWeekRatioLoss = value;
                NotifyOfPropertyChange(() => ThisWeekRatioLoss);
                NotifyOfPropertyChange(() => ThisWeekRatio);
            }
        }

        public decimal ThisWeekRatio
        {
            get
            {
                if (ThisWeekRatioLoss == 0)
                {
                    return 0;
                }
                return Math.Round(ThisWeekRatioWin / ThisWeekRatioLoss, 3);
            }
        }

        public decimal ThisMonthRatioWin
        {
            get { return thisMonthRatioWin; }
            set
            {
                if (value == thisMonthRatioWin)
                {
                    return;
                }
                thisMonthRatioWin = value;
                NotifyOfPropertyChange(() => ThisMonthRatioWin);
                NotifyOfPropertyChange(() => ThisMonthRatio);
            }
        }

        public decimal ThisMonthRatioLoss
        {
            get { return thisMonthRatioLoss; }
            set
            {
                if (value == thisMonthRatioLoss)
                {
                    return;
                }
                thisMonthRatioLoss = value;
                NotifyOfPropertyChange(() => ThisMonthRatioLoss);
                NotifyOfPropertyChange(() => ThisMonthRatio);
            }
        }

        public decimal ThisMonthRatio
        {
            get
            {
                if (ThisMonthRatioLoss == 0)
                {
                    return 0;
                }
                return Math.Round(ThisMonthRatioWin / ThisMonthRatioLoss, 3);
            }
        }

        public decimal Last7DaysRatioWin
        {
            get { return last7DaysRatioWin; }
            set
            {
                if (value == last7DaysRatioWin)
                {
                    return;
                }
                last7DaysRatioWin = value;
                NotifyOfPropertyChange(() => Last7DaysRatioWin);
                NotifyOfPropertyChange(() => Last7DaysRatio);
            }
        }

        public decimal Last7DaysRatioLoss
        {
            get { return last7DaysRatioLoss; }
            set
            {
                if (value == last7DaysRatioLoss)
                {
                    return;
                }
                last7DaysRatioLoss = value;
                NotifyOfPropertyChange(() => Last7DaysRatioLoss);
                NotifyOfPropertyChange(() => Last7DaysRatio);
            }
        }

        public decimal Last7DaysRatio
        {
            get
            {
                if (Last7DaysRatioLoss == 0)
                {
                    return 0;
                }
                return Math.Round(Last7DaysRatioWin / Last7DaysRatioLoss, 3);
            }
        }

        public decimal Last30DaysRatioWin
        {
            get { return last30DaysRatioWin; }
            set
            {
                if (value == last30DaysRatioWin)
                {
                    return;
                }
                last30DaysRatioWin = value;
                NotifyOfPropertyChange(() => Last30DaysRatioWin);
                NotifyOfPropertyChange(() => Last30DaysRatio);
            }
        }

        public decimal Last30DaysRatioLoss
        {
            get { return last30DaysRatioLoss; }
            set
            {
                if (value == last30DaysRatioLoss)
                {
                    return;
                }
                last30DaysRatioLoss = value;
                NotifyOfPropertyChange(() => Last30DaysRatioLoss);
                NotifyOfPropertyChange(() => Last30DaysRatio);
            }
        }

        public decimal Last30DaysRatio
        {
            get
            {
                if (Last30DaysRatioLoss == 0)
                {
                    return 0;
                }
                return Math.Round(Last30DaysRatioWin / Last30DaysRatioLoss, 3);
            }
        }

        public void RefreshFrom(Func<HearthStatsDbContext> contextFactory, Expression<Func<GameResult, bool>> filter)
        {
            if (refreshing)
            {
                return;
            }
            refreshing = true;
            if (!initialized)
            {
                InitializeData();
                initialized = true;
            }

            //Task.WaitAll(
            //    Task.Run(() => CalculateWinsAndLosses(contextFactory(), filter)),
            //    Task.Run(() => CalculateHeroesPlayed(contextFactory(), filter)),
            //    Task.Run(() => CalculateRatios(contextFactory()))
            //    );

            CalculateWinsAndLosses(contextFactory(), filter);
            CalculateHeroesPlayed(contextFactory(), filter);
            CalculateRatios(contextFactory());

            refreshing = false;
        }

        private void CalculateRatios(HearthStatsDbContext context)
        {
            var now = DateTime.Now;
            var start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            decimal numGames;
            decimal numWon;
            decimal numLoss;
            GetStatsSince(context, start, out numGames, out numWon, out numLoss);

            if (numGames > 0)
            {
                TodayRatioWin = Math.Round((numWon / numGames) * 100, 0);
                TodayRatioLoss = Math.Round((numLoss / numGames) * 100, 0);
            }
            else
            {
                TodayRatioWin = 0;
                TodayRatioLoss = 0;
            }

            start = now.StartOfWeek(DayOfWeek.Monday);
            GetStatsSince(context, start, out numGames, out numWon, out numLoss);
            if (numGames > 0)
            {
                ThisWeekRatioWin = Math.Round((numWon / numGames) * 100, 0);
                ThisWeekRatioLoss = Math.Round((numLoss / numGames) * 100, 0);
            }
            else
            {
                ThisWeekRatioWin = 0;
                ThisWeekRatioLoss = 0;
            }

            start = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
            GetStatsSince(context, start, out numGames, out numWon, out numLoss);
            if (numGames > 0)
            {
                ThisMonthRatioWin = Math.Round((numWon / numGames) * 100, 0);
                ThisMonthRatioLoss = Math.Round((numLoss / numGames) * 100, 0);
            }
            else
            {
                ThisMonthRatioWin = 0;
                ThisMonthRatioLoss = 0;
            }

            start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            start = start.AddDays(-7);
            GetStatsSince(context, start, out numGames, out numWon, out numLoss);
            if (numGames > 0)
            {
                Last7DaysRatioWin = Math.Round((numWon / numGames) * 100, 0);
                Last7DaysRatioLoss = Math.Round((numLoss / numGames) * 100, 0);
            }
            else
            {
                Last7DaysRatioWin = 0;
                Last7DaysRatioLoss = 0;
            }

            start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            start = start.AddDays(-30);
            GetStatsSince(context, start, out numGames, out numWon, out numLoss);
            if (numGames > 0)
            {
                Last30DaysRatioWin = Math.Round((numWon / numGames) * 100, 0);
                Last30DaysRatioLoss = Math.Round((numLoss / numGames) * 100, 0);
            }
            else
            {
                Last30DaysRatioWin = 0;
                Last30DaysRatioLoss = 0;
            }
        }

        private void GetStatsSince(HearthStatsDbContext context, DateTime start, out decimal numGames, out decimal numWon, out decimal numLoss)
        {
            GameMode gameMode;
            var filterGameMode = Enum.TryParse(LatestGamesViewModel.FilterGameMode, out gameMode);
            var heroId = Guid.Empty;
            var filterHero = LatestGamesViewModel.FilterHero != null && !String.IsNullOrEmpty(LatestGamesViewModel.FilterHero.Key);
            if (filterHero)
            {
                heroId = LatestGamesViewModel.FilterHero.Id;
            }
            var oppHeroId = Guid.Empty;
            var filterOppHero = LatestGamesViewModel.FilterOpponentHero != null && !String.IsNullOrEmpty(LatestGamesViewModel.FilterOpponentHero.Key);
            if (filterOppHero)
            {
                oppHeroId = LatestGamesViewModel.FilterOpponentHero.Id;
            }
            var deckId = Guid.Empty;
            var filterDeck = LatestGamesViewModel.FilterDeck != null && !String.IsNullOrEmpty(LatestGamesViewModel.FilterDeck.Key);
            if (filterDeck)
            {
                deckId = LatestGamesViewModel.FilterDeck.Id;
            }

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

        private void InitializeData()
        {
            var data = GlobalData.Get();
            heroes.Clear();
            heroes.AddRange(data.Heroes);
        }

        private void CalculateHeroesPlayed(HearthStatsDbContext context, Expression<Func<GameResult, bool>> filter)
        {
            var games = context.Games;

            heroesPlayed.Clear();
            opponentHeroesPlayed.Clear();
            var total = games.Where(filter).Count();
            if (total == 0)
            {
                return;
            }
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

            heroesPlayed.IsNotifying = false;
            foreach (var hero in heroestats)
            {
                // float count = games.Where(filter).Count(x => x.Hero != null && x.Hero.Key == hero.Key);
                if (hero.Count > 0)
                {
                    heroesPlayed.Add(new StatModel(string.Format("{0}: {1}", hero.ClassName, hero.Count), (float)hero.Count / total * 100, hero.Key.GetBrush()));
                }
            }
            heroesPlayed.IsNotifying = true;
            heroesPlayed.Refresh();

            opponentHeroesPlayed.IsNotifying = false;
            foreach (var hero in oppheroestats)
            {
                //count = games.Where(filter).Count(x => x.OpponentHero != null && x.OpponentHero.Key == hero.Key);
                if (hero.Count > 0)
                {
                    opponentHeroesPlayed.Add(new StatModel(string.Format("{0}: {1}", hero.ClassName, hero.Count), (float)hero.Count / total * 100, hero.Key.GetBrush()));
                }
            }
            opponentHeroesPlayed.IsNotifying = true;
            opponentHeroesPlayed.Refresh();
        }

        private void CalculateWinsAndLosses(HearthStatsDbContext context, Expression<Func<GameResult, bool>> filter)
        {
            var games = context.Games;
            float total = games.Where(filter).Count();
            float winsC = games.Where(filter).Count(x => x.Victory && !x.GoFirst);
            float lossesC = games.Where(filter).Count(x => !x.Victory && !x.GoFirst);
            float winsNC = games.Where(filter).Count(x => x.Victory && x.GoFirst);
            float lossesNC = games.Where(filter).Count(x => !x.Victory && x.GoFirst);
            var wins = winsC + winsNC;
            var losses = lossesC + lossesNC;

            WinsAndLosses.Clear();
            Wins.Clear();
            Losses.Clear();
            WithCoin.Clear();
            WithoutCoin.Clear();
            if (total <= 0)
            {
                WinsAndLosses.Add(new StatModel("Wins", 0));
                WinsAndLosses.Add(new StatModel("Losses", 0));
                Wins.Add(new StatModel("Coin", 0));
                Wins.Add(new StatModel("No coin", 0));
                Losses.Add(new StatModel("Coin", 0));
                Losses.Add(new StatModel("No coin", 0));
                WithCoin.Add(new StatModel("Losses", 0));
                WithCoin.Add(new StatModel("Losses", 0));
                WithoutCoin.Add(new StatModel("Losses", 0));
                WithoutCoin.Add(new StatModel("Losses", 0));

                return;
            }

            WinsAndLosses.Add(new StatModel(string.Format("Wins: {0}", wins), wins / total * 100));
            WinsAndLosses.Add(new StatModel(string.Format("Losses: {0}", losses), losses / total * 100));

            Wins.Add(new StatModel(string.Format("Coin: {0}", winsC), winsC / wins * 100));
            Wins.Add(new StatModel(string.Format("No coin: {0}", winsNC), winsNC / wins * 100));

            Losses.Add(new StatModel(string.Format("Coin: {0}", lossesC), lossesC / losses * 100));
            Losses.Add(new StatModel(string.Format("No coin: {0}", lossesNC), lossesNC / losses * 100));

            WithCoin.Add(new StatModel(string.Format("Wins: {0}", winsC), winsC / (winsC + lossesC) * 100));
            WithCoin.Add(new StatModel(string.Format("Losses: {0}", lossesC), lossesC / (winsC + lossesC) * 100));

            WithoutCoin.Add(new StatModel(string.Format("Wins: {0}", winsNC), winsNC / (winsNC + lossesNC) * 100));
            WithoutCoin.Add(new StatModel(string.Format("Losses: {0}", lossesNC), lossesNC / (winsNC + lossesNC) * 100));
        }
    }
}
