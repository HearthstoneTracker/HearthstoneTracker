using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;
using HearthCap.Data;
using HearthCap.Features.Core;

namespace HearthCap.Features.ArenaSessions.Statistics
{
    [Export(typeof(FilteredStatsViewModel))]
    public class FilteredStatsViewModel : Screen
    {
        private bool initialized;
        private readonly BindableCollection<StatModel> winsAndLosses = new BindableCollection<StatModel>();

        private readonly BindableCollection<StatModel> heroesPlayed = new BindableCollection<StatModel>();

        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        private readonly BindableCollection<StatModel> winRatios = new BindableCollection<StatModel>();

        private bool refreshing;

        private readonly BindableCollection<StatModel> opponentHeroesPlayed = new BindableCollection<StatModel>();

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

        private void AddDesignModeData()
        {
            WinsAndLosses.Add(new StatModel("Wins", 30));
            WinsAndLosses.Add(new StatModel("Losses", 70));

            WinRatios.Add(new StatModel("0-6", 10));
            WinRatios.Add(new StatModel("7+", 14));
            WinRatios.Add(new StatModel("12", 3));
        }

        public BindableCollection<StatModel> WinsAndLosses
        {
            get { return winsAndLosses; }
        }

        public BindableCollection<StatModel> HeroesPlayed
        {
            get { return heroesPlayed; }
        }

        public BindableCollection<StatModel> OpponentHeroesPlayed
        {
            get { return opponentHeroesPlayed; }
        }

        public BindableCollection<StatModel> WinRatios
        {
            get { return winRatios; }
        }

        public void RefreshFrom(Func<HearthStatsDbContext> context, Expression<Func<ArenaSession, bool>> filter)
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
            //    Task.Run(() => CalculateWinsAndLosses(context(), filter)),
            //    Task.Run(() => CalculateWinRatios(context(), filter)),
            //    Task.Run(() => CalculateHeroesPlayed(context(), filter)),
            //    Task.Run(() => CalculateOppHeroesPlayed(context(), filter))
            //    ); 

            CalculateWinsAndLosses(context(), filter);
            CalculateWinRatios(context(), filter);
            CalculateWinRatios(context(), filter);
            CalculateHeroesPlayed(context(), filter);
            CalculateOppHeroesPlayed(context(), filter);

            refreshing = false;
        }

        private void CalculateWinRatios(HearthStatsDbContext context, Expression<Func<ArenaSession, bool>> filter)
        {
            var arenas = context.ArenaSessions.Where(filter);
            // float total = results.Sum(a => a.Games.Count);
            float wins2 = arenas.Count(a => a.Wins <= 2);
            float wins3 = arenas.Count(a => a.Wins == 3);
            float wins4 = arenas.Count(a => a.Wins >= 4 && a.Wins <= 6);
            float wins7 = arenas.Count(a => a.Wins >= 7 && a.Wins <= 11);
            float wins12 = arenas.Count(a => a.Wins == 12);

            WinRatios.IsNotifying = false;
            WinRatios.Clear();
            WinRatios.Add(new StatModel("0-2", wins2));
            WinRatios.Add(new StatModel("3", wins3));
            WinRatios.Add(new StatModel("4-6", wins4));
            WinRatios.Add(new StatModel("7-11", wins7));
            WinRatios.Add(new StatModel("12", wins12));
            WinRatios.IsNotifying = true;
            WinRatios.Refresh();
        }

        private void InitializeData()
        {
            var data = GlobalData.Get();
            heroes.Clear();
            heroes.AddRange(data.Heroes);
        }

        private void CalculateOppHeroesPlayed(HearthStatsDbContext context, Expression<Func<ArenaSession, bool>> filter)
        {
            var arenas = context.ArenaSessions.Where(filter);
            opponentHeroesPlayed.IsNotifying = false;
            opponentHeroesPlayed.Clear();
            var total = arenas.Count();
            if (total == 0)
            {
                return;
            }
            var gamesCount = context.Games.Count(x => arenas.Contains(x.ArenaSession));

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
                    opponentHeroesPlayed.Add(new StatModel(string.Format("{0}: {1}", hero.ClassName, hero.Count), (float)hero.Count / gamesCount * 100, hero.Key.GetBrush()));
                }
            }
            opponentHeroesPlayed.IsNotifying = true;
            opponentHeroesPlayed.Refresh();
        }

        private void CalculateHeroesPlayed(HearthStatsDbContext context, Expression<Func<ArenaSession, bool>> filter)
        {
            var arenas = context.ArenaSessions.Where(filter);
            heroesPlayed.IsNotifying = false;
            heroesPlayed.Clear();

            var total = arenas.Count();
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
                    heroesPlayed.Add(new StatModel(string.Format("{0}: {1}", hero.ClassName, hero.Count), (float)hero.Count / total * 100, hero.Key.GetBrush()));
                }
            }

            heroesPlayed.IsNotifying = true;
            heroesPlayed.Refresh();
        }

        private void CalculateWinsAndLosses(HearthStatsDbContext context, Expression<Func<ArenaSession, bool>> filter)
        {
            var arenas = context.ArenaSessions.Where(filter);
            if (!arenas.Any())
            {
                WinsAndLosses.Add(new StatModel("Wins", 0));
                WinsAndLosses.Add(new StatModel("Losses", 0));
                return;
            }

            float total = arenas.Sum(a => a.Wins + a.Losses);
            float wins = arenas.Sum(a => a.Wins);
            float losses = arenas.Sum(a => a.Losses);
            WinsAndLosses.Clear();
            if (total <= 0)
            {
                WinsAndLosses.Add(new StatModel("Wins", 0));
                WinsAndLosses.Add(new StatModel("Losses", 0));
                return;
            }

            WinsAndLosses.Add(new StatModel(string.Format("Wins: {0}", wins), wins / total * 100));
            WinsAndLosses.Add(new StatModel(string.Format("Losses: {0}", losses), losses / total * 100));
        }
    }
}
