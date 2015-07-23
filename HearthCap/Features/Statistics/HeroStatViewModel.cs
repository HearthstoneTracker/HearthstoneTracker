namespace HearthCap.Features.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Util;

    [Export(typeof(IStatsViewModel))]
    public class HeroStatViewModel : StatViewModelBase
    {
        private readonly IRepository<GameResult> gameRepository;

        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();
        private readonly BindableCollection<IDictionary<Hero, IList<StatModel>>> heroStats = new BindableCollection<IDictionary<Hero, IList<StatModel>>>();

        private bool refreshing;

        [ImportingConstructor]
        public HeroStatViewModel(IRepository<GameResult> gameRepository)
        {
            this.gameRepository = gameRepository;
        }

        [Import]
        public GlobalData GlobalData { get; set; }

        public IObservableCollection<Hero> Heroes
        {
            get
            {
                return this.heroes;
            }
        }


        public BindableCollection<IDictionary<Hero, IList<StatModel>>> HeroStats
        {
            get
            {
                return this.heroStats;
            }
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override async void OnInitialize()
        {
            await this.LoadData();
        }

        private async Task LoadData()
        {
            var data = await this.GlobalData.GetAsync();
            this.heroes.Add(new Hero(null));
            this.heroes.AddRange(data.Heroes);
        }

        public override void RefreshData()
        {
            if (refreshing) return;
            refreshing = true;
            var newstats = new BindableCollection<IDictionary<Hero, IList<StatModel>>>();
            Task.Run(
                () =>
                {
                    using (Busy.GetTicket())
                    {
                        foreach (Hero hero in Heroes)
                        {
                            if (hero.Key == null)
                                continue;

                            var r =
                                gameRepository.Query(
                                    x =>
                                    {
                                        var q = x.Where(g => g.Hero.Id == hero.Id);
                                        q = q.Where(GetFilterExpression());
                                        var group = q
                                            .GroupBy(g => g.OpponentHero.Key)
                                            .Select(
                                                g =>
                                                new
                                                    {
                                                        HeroKey = g.Key,
                                                        GlobalTotal = q.Count(),
                                                        Total = g.Count(),
                                                        TotalCoin = g.Count(c => !c.GoFirst),
                                                        TotalNoCoin = g.Count(c => c.GoFirst),
                                                        Wins = g.Count(c => c.Victory),
                                                        Losses = g.Count(c => !c.Victory),
                                                        WinsCoin = g.Count(c => c.Victory && !c.GoFirst),
                                                        WinsNoCoin = g.Count(c => c.Victory && c.GoFirst),
                                                        LossesCoin = g.Count(c => !c.Victory && !c.GoFirst),
                                                        LossesNoCoin = g.Count(c => !c.Victory && c.GoFirst)
                                                    });
                                        return group.ToList();
                                    });
                            var stats = new Dictionary<Hero, IList<StatModel>>();
                            newstats.Add(stats);
                            var lst = new List<StatModel>();
                            stats.Add(hero, lst);
                            for (int i = 0; i < Heroes.Count; i++)
                            {
                                var oppHero = Heroes[i];
                                dynamic result = null;
                                if (oppHero.Key != null)
                                {
                                    result = r.FirstOrDefault(x => x.HeroKey == oppHero.Key);
                                }
                                else
                                {
                                    result = new
                                                 {
                                                     HeroKey = String.Empty,
                                                     GlobalTotal = r.Sum(x => x.Total),
                                                     Total = r.Sum(x => x.Total),
                                                     TotalCoin = r.Sum(x => x.TotalCoin),
                                                     TotalNoCoin = r.Sum(x => x.TotalNoCoin),
                                                     Wins = r.Sum(x => x.Wins),
                                                     Losses = r.Sum(x => x.Losses),
                                                     WinsCoin = r.Sum(x => x.WinsCoin),
                                                     WinsNoCoin = r.Sum(x => x.WinsNoCoin),
                                                     LossesCoin = r.Sum(x => x.LossesCoin),
                                                     LossesNoCoin = r.Sum(x => x.LossesNoCoin)
                                                 };
                                }

                                if (result == null)
                                {
                                    lst.Add(new StatModel() { Hero = oppHero });
                                }
                                else
                                {
                                    lst.Add(
                                        new StatModel()
                                            {
                                                Hero = oppHero,
                                                TotalGames = result.Total,
                                                GlobalTotal = result.GlobalTotal,
                                                Wins = result.Wins,
                                                Losses = result.Losses,
                                                WinsCoin = result.WinsCoin,
                                                WinsNoCoin = result.WinsNoCoin,
                                                LossesCoin = result.LossesCoin,
                                                LossesNoCoin = result.LossesNoCoin,
                                                WinRate = result.Total > 0 ? Math.Round((decimal)result.Wins / result.Total * 100, 0) : 0,
                                                LossRate = result.Total > 0 ? Math.Round((decimal)result.Losses / result.Total * 100, 0) : 0,
                                                WinRateCoin =
                                                    result.TotalCoin > 0 ? Math.Round((decimal)result.WinsCoin / result.TotalCoin * 100, 0) : 0,
                                                WinRateNoCoin =
                                                    result.TotalNoCoin > 0 ? Math.Round((decimal)result.WinsNoCoin / result.TotalNoCoin * 100, 0) : 0,
                                                LossRateCoin =
                                                    result.TotalCoin > 0 ? Math.Round((decimal)result.LossesCoin / result.TotalCoin * 100, 0) : 0,
                                                LossRateNoCoin =
                                                    result.TotalNoCoin > 0 ? Math.Round((decimal)result.LossesNoCoin / result.TotalNoCoin * 100, 0) : 0
                                            });
                                }
                            }
                        }
                        refreshing = false;
                    }
                }).ContinueWith(
                    t =>
                    {
                        using (Busy.GetTicket())
                        {
                            HeroStats.IsNotifying = false;
                            HeroStats.Clear();
                            HeroStats.AddRange(newstats);
                            HeroStats.IsNotifying = true;
                            HeroStats.Refresh();
                        }
                    });
        }
    }
}