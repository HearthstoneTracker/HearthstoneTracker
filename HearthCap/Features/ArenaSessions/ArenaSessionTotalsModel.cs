namespace HearthCap.Features.ArenaSessions
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Games.Models;

    public class ArenaSessionTotalsModel : PropertyChangedBase
    {
        private int arenas;

        private int zeroToSix;

        private int sevenPlus;

        private int hours;

        private int minutes;

        private int totalArenas;

        private int totalHours;

        private int totalMinutes;

        private int twelve;

        private int goldEarned;

        private int dustEarned;

        private int packsEarned;

        private decimal averageWins;

        public ArenaSessionTotalsModel()
        {
        }

        public int Arenas
        {
            get
            {
                return this.arenas;
            }
            set
            {
                if (value == this.arenas)
                {
                    return;
                }
                this.arenas = value;
                this.NotifyOfPropertyChange(() => this.Arenas);
            }
        }

        public int ZeroToSix
        {
            get
            {
                return this.zeroToSix;
            }
            set
            {
                if (value == this.zeroToSix)
                {
                    return;
                }
                this.zeroToSix = value;
                this.NotifyOfPropertyChange(() => this.ZeroToSix);
            }
        }

        public int SevenPlus
        {
            get
            {
                return this.sevenPlus;
            }
            set
            {
                if (value == this.sevenPlus)
                {
                    return;
                }
                this.sevenPlus = value;
                this.NotifyOfPropertyChange(() => this.SevenPlus);
            }
        }

        public int Twelve
        {
            get
            {
                return this.twelve;
            }
            set
            {
                if (value == this.twelve)
                {
                    return;
                }
                this.twelve = value;
                this.NotifyOfPropertyChange(() => this.Twelve);
            }
        }

        public int Hours
        {
            get
            {
                return this.hours;
            }
            set
            {
                if (value == this.hours)
                {
                    return;
                }
                this.hours = value;
                this.NotifyOfPropertyChange(() => this.Hours);
            }
        }

        public int Minutes
        {
            get
            {
                return this.minutes;
            }
            set
            {
                if (value == this.minutes)
                {
                    return;
                }
                this.minutes = value;
                this.NotifyOfPropertyChange(() => this.Minutes);
            }
        }

        public int TotalArenas
        {
            get
            {
                return this.totalArenas;
            }
            set
            {
                if (value == this.totalArenas)
                {
                    return;
                }
                this.totalArenas = value;
                this.NotifyOfPropertyChange(() => this.TotalArenas);
            }
        }

        public int TotalHours
        {
            get
            {
                return this.totalHours;
            }
            set
            {
                if (value == this.totalHours)
                {
                    return;
                }
                this.totalHours = value;
                this.NotifyOfPropertyChange(() => this.TotalHours);
            }
        }

        public int TotalMinutes
        {
            get
            {
                return this.totalMinutes;
            }
            set
            {
                if (value == this.totalMinutes)
                {
                    return;
                }
                this.totalMinutes = value;
                this.NotifyOfPropertyChange(() => this.TotalMinutes);
            }
        }

        public int GoldEarned
        {
            get
            {
                return this.goldEarned;
            }
            set
            {
                if (value == this.goldEarned)
                {
                    return;
                }
                this.goldEarned = value;
                this.NotifyOfPropertyChange(() => this.GoldEarned);
            }
        }

        public int DustEarned
        {
            get
            {
                return this.dustEarned;
            }
            set
            {
                if (value == this.dustEarned)
                {
                    return;
                }
                this.dustEarned = value;
                this.NotifyOfPropertyChange(() => this.DustEarned);
            }
        }

        public int PacksEarned
        {
            get
            {
                return this.packsEarned;
            }
            set
            {
                if (value == this.packsEarned)
                {
                    return;
                }
                this.packsEarned = value;
                this.NotifyOfPropertyChange(() => this.PacksEarned);
            }
        }

        public decimal AverageWins
        {
            get
            {
                return this.averageWins;
            }
            set
            {
                if (value == this.averageWins)
                {
                    return;
                }

                this.averageWins = value;
                NotifyOfPropertyChange(() => this.AverageWins);
            }
        }

        public void Update(Func<HearthStatsDbContext> dbContext, Expression<Func<ArenaSession, bool>> filter)
        {
            using (var context = dbContext())
            {
                var arenas = context.ArenaSessions.Where(filter);

                this.Arenas = arenas.Count();
                ZeroToSix = arenas.Count(x => x.Wins < 7);
                SevenPlus = arenas.Count(x => x.Wins >= 7);
                Twelve = arenas.Count(x => x.Wins == 12);
                // TODO: find a more optimized way of querying this
                //var totalMinutes = arenas
                //    .Select(x => new { x.StartDate, x.EndDate })
                //    .ToList()
                //    .Sum(x => x.EndDate == null ? 0 : x.EndDate.Value.Subtract(x.StartDate).Minutes);

                var totalMinutesV = context.Games
                    .Where(g => arenas.Contains(g.ArenaSession))
                    .Sum(g => DbFunctions.DiffMinutes(g.Started, g.Stopped));
                var totalMinutes = totalMinutesV ?? 0;
                this.Hours = totalMinutes / 60;
                this.Minutes = totalMinutes % 60;

                this.TotalArenas = context.ArenaSessions.Count();
                var totaltotalminutes = context.Database.SqlQuery<int?>("SELECT SUM(DATEDIFF(mi, [Started], [Stopped])) FROM GameResults WHERE ArenaSessionId IS NOT NULL").FirstOrDefault();
                if (totaltotalminutes.HasValue)
                {
                    this.TotalHours = totaltotalminutes.Value / 60;
                    this.TotalMinutes = totaltotalminutes.Value % 60;
                }

                this.GoldEarned = arenas.Sum(x => (int?)x.RewardGold) ?? 0;
                this.DustEarned = arenas.Sum(x => (int?)x.RewardDust) ?? 0;
                this.PacksEarned = arenas.Sum(x => (int?)x.RewardPacks) ?? 0;

                if (Arenas > 0)
                {
                    var wins = arenas.Sum(x => (int?)x.Wins) ?? 0;
                    this.AverageWins = Math.Round(wins / (decimal)Arenas, 1);
                }
            }
        }
    }
}