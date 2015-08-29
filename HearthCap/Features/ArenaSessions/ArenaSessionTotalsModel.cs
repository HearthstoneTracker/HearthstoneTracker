using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;
using HearthCap.Data;

namespace HearthCap.Features.ArenaSessions
{
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

        public int Arenas
        {
            get { return arenas; }
            set
            {
                if (value == arenas)
                {
                    return;
                }
                arenas = value;
                NotifyOfPropertyChange(() => Arenas);
            }
        }

        public int ZeroToSix
        {
            get { return zeroToSix; }
            set
            {
                if (value == zeroToSix)
                {
                    return;
                }
                zeroToSix = value;
                NotifyOfPropertyChange(() => ZeroToSix);
            }
        }

        public int SevenPlus
        {
            get { return sevenPlus; }
            set
            {
                if (value == sevenPlus)
                {
                    return;
                }
                sevenPlus = value;
                NotifyOfPropertyChange(() => SevenPlus);
            }
        }

        public int Twelve
        {
            get { return twelve; }
            set
            {
                if (value == twelve)
                {
                    return;
                }
                twelve = value;
                NotifyOfPropertyChange(() => Twelve);
            }
        }

        public int Hours
        {
            get { return hours; }
            set
            {
                if (value == hours)
                {
                    return;
                }
                hours = value;
                NotifyOfPropertyChange(() => Hours);
            }
        }

        public int Minutes
        {
            get { return minutes; }
            set
            {
                if (value == minutes)
                {
                    return;
                }
                minutes = value;
                NotifyOfPropertyChange(() => Minutes);
            }
        }

        public int TotalArenas
        {
            get { return totalArenas; }
            set
            {
                if (value == totalArenas)
                {
                    return;
                }
                totalArenas = value;
                NotifyOfPropertyChange(() => TotalArenas);
            }
        }

        public int TotalHours
        {
            get { return totalHours; }
            set
            {
                if (value == totalHours)
                {
                    return;
                }
                totalHours = value;
                NotifyOfPropertyChange(() => TotalHours);
            }
        }

        public int TotalMinutes
        {
            get { return totalMinutes; }
            set
            {
                if (value == totalMinutes)
                {
                    return;
                }
                totalMinutes = value;
                NotifyOfPropertyChange(() => TotalMinutes);
            }
        }

        public int GoldEarned
        {
            get { return goldEarned; }
            set
            {
                if (value == goldEarned)
                {
                    return;
                }
                goldEarned = value;
                NotifyOfPropertyChange(() => GoldEarned);
            }
        }

        public int DustEarned
        {
            get { return dustEarned; }
            set
            {
                if (value == dustEarned)
                {
                    return;
                }
                dustEarned = value;
                NotifyOfPropertyChange(() => DustEarned);
            }
        }

        public int PacksEarned
        {
            get { return packsEarned; }
            set
            {
                if (value == packsEarned)
                {
                    return;
                }
                packsEarned = value;
                NotifyOfPropertyChange(() => PacksEarned);
            }
        }

        public decimal AverageWins
        {
            get { return averageWins; }
            set
            {
                if (value == averageWins)
                {
                    return;
                }

                averageWins = value;
                NotifyOfPropertyChange(() => AverageWins);
            }
        }

        public void Update(Func<HearthStatsDbContext> dbContext, Expression<Func<ArenaSession, bool>> filter)
        {
            using (var context = dbContext())
            {
                var arenas = context.ArenaSessions.Where(filter);

                Arenas = arenas.Count();
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
                Hours = totalMinutes / 60;
                Minutes = totalMinutes % 60;

                TotalArenas = context.ArenaSessions.Count();
                var totaltotalminutes = context.Database.SqlQuery<int?>("SELECT SUM(DATEDIFF(mi, [Started], [Stopped])) FROM GameResults WHERE ArenaSessionId IS NOT NULL").FirstOrDefault();
                if (totaltotalminutes.HasValue)
                {
                    TotalHours = totaltotalminutes.Value / 60;
                    TotalMinutes = totaltotalminutes.Value % 60;
                }

                GoldEarned = arenas.Sum(x => (int?)x.RewardGold) ?? 0;
                DustEarned = arenas.Sum(x => (int?)x.RewardDust) ?? 0;
                PacksEarned = arenas.Sum(x => (int?)x.RewardPacks) ?? 0;

                if (Arenas > 0)
                {
                    var wins = arenas.Sum(x => (int?)x.Wins) ?? 0;
                    AverageWins = Math.Round(wins / (decimal)Arenas, 1);
                }
            }
        }
    }
}
