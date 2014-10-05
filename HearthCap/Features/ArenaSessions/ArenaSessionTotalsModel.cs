// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaSessionTotalsModel.cs" company="">
//   
// </copyright>
// <summary>
//   The arena session totals model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.ArenaSessions
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    using HearthCap.Data;

    /// <summary>
    /// The arena session totals model.
    /// </summary>
    public class ArenaSessionTotalsModel : PropertyChangedBase
    {
        /// <summary>
        /// The arenas.
        /// </summary>
        private int arenas;

        /// <summary>
        /// The zero to six.
        /// </summary>
        private int zeroToSix;

        /// <summary>
        /// The seven plus.
        /// </summary>
        private int sevenPlus;

        /// <summary>
        /// The hours.
        /// </summary>
        private int hours;

        /// <summary>
        /// The minutes.
        /// </summary>
        private int minutes;

        /// <summary>
        /// The total arenas.
        /// </summary>
        private int totalArenas;

        /// <summary>
        /// The total hours.
        /// </summary>
        private int totalHours;

        /// <summary>
        /// The total minutes.
        /// </summary>
        private int totalMinutes;

        /// <summary>
        /// The twelve.
        /// </summary>
        private int twelve;

        /// <summary>
        /// The gold earned.
        /// </summary>
        private int goldEarned;

        /// <summary>
        /// The dust earned.
        /// </summary>
        private int dustEarned;

        /// <summary>
        /// The packs earned.
        /// </summary>
        private int packsEarned;

        /// <summary>
        /// The average wins.
        /// </summary>
        private decimal averageWins;

        /// <summary>
        /// Gets or sets the arenas.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the zero to six.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the seven plus.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the twelve.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the hours.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the minutes.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the total arenas.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the total hours.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the total minutes.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the gold earned.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the dust earned.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the packs earned.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the average wins.
        /// </summary>
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
                this.NotifyOfPropertyChange(() => this.AverageWins);
            }
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        public void Update(Func<HearthStatsDbContext> dbContext, Expression<Func<ArenaSession, bool>> filter)
        {
            using (var context = dbContext())
            {
                var arenas = context.ArenaSessions.Where(filter);

                this.Arenas = arenas.Count();
                this.ZeroToSix = arenas.Count(x => x.Wins < 7);
                this.SevenPlus = arenas.Count(x => x.Wins >= 7);
                this.Twelve = arenas.Count(x => x.Wins == 12);

                // TODO: find a more optimized way of querying this
                // var totalMinutes = arenas
                // .Select(x => new { x.StartDate, x.EndDate })
                // .ToList()
                // .Sum(x => x.EndDate == null ? 0 : x.EndDate.Value.Subtract(x.StartDate).Minutes);
                var totalMinutesV = context.Games.Where(g => arenas.Contains(g.ArenaSession)).Sum(g => DbFunctions.DiffMinutes(g.Started, g.Stopped));
                var totalMinutes = totalMinutesV ?? 0;
                this.Hours = totalMinutes / 60;
                this.Minutes = totalMinutes % 60;

                this.TotalArenas = context.ArenaSessions.Count();
                var totaltotalminutes =
                    context.Database.SqlQuery<int?>(
                        "SELECT SUM(DATEDIFF(mi, [Started], [Stopped])) FROM GameResults WHERE ArenaSessionId IS NOT NULL").FirstOrDefault();
                if (totaltotalminutes.HasValue)
                {
                    this.TotalHours = totaltotalminutes.Value / 60;
                    this.TotalMinutes = totaltotalminutes.Value % 60;
                }

                this.GoldEarned = arenas.Sum(x => (int?)x.RewardGold) ?? 0;
                this.DustEarned = arenas.Sum(x => (int?)x.RewardDust) ?? 0;
                this.PacksEarned = arenas.Sum(x => (int?)x.RewardPacks) ?? 0;

                if (this.Arenas > 0)
                {
                    var wins = arenas.Sum(x => (int?)x.Wins) ?? 0;
                    this.AverageWins = Math.Round(wins / (decimal)this.Arenas, 1);
                }
            }
        }
    }
}