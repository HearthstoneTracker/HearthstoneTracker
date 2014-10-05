// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameResultTotalsModel.cs" company="">
//   
// </copyright>
// <summary>
//   The game result totals model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games.LatestGames
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    using HearthCap.Data;

    /// <summary>
    /// The game result totals model.
    /// </summary>
    public class GameResultTotalsModel : PropertyChangedBase
    {
        /// <summary>
        /// The games.
        /// </summary>
        private int games;

        /// <summary>
        /// The won.
        /// </summary>
        private int won;

        /// <summary>
        /// The lost.
        /// </summary>
        private int lost;

        /// <summary>
        /// The hours.
        /// </summary>
        private int hours;

        /// <summary>
        /// The minutes.
        /// </summary>
        private int minutes;

        /// <summary>
        /// The total games.
        /// </summary>
        private int totalGames;

        /// <summary>
        /// The total hours.
        /// </summary>
        private int totalHours;

        /// <summary>
        /// The total minutes.
        /// </summary>
        private int totalMinutes;

        /// <summary>
        /// Gets or sets the games.
        /// </summary>
        public int Games
        {
            get
            {
                return this.games;
            }

            set
            {
                if (value == this.games)
                {
                    return;
                }

                this.games = value;
                this.NotifyOfPropertyChange(() => this.Games);
            }
        }

        /// <summary>
        /// Gets or sets the won.
        /// </summary>
        public int Won
        {
            get
            {
                return this.won;
            }

            set
            {
                if (value == this.won)
                {
                    return;
                }

                this.won = value;
                this.NotifyOfPropertyChange(() => this.Won);
            }
        }

        /// <summary>
        /// Gets or sets the lost.
        /// </summary>
        public int Lost
        {
            get
            {
                return this.lost;
            }

            set
            {
                if (value == this.lost)
                {
                    return;
                }

                this.lost = value;
                this.NotifyOfPropertyChange(() => this.Lost);
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
        /// Gets or sets the total games.
        /// </summary>
        public int TotalGames
        {
            get
            {
                return this.totalGames;
            }

            set
            {
                if (value == this.totalGames)
                {
                    return;
                }

                this.totalGames = value;
                this.NotifyOfPropertyChange(() => this.TotalGames);
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
        /// The update.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        public void Update(Func<HearthStatsDbContext> dbContext, Expression<Func<GameResult, bool>> filter)
        {
            using (var context = dbContext())
            {
                var result = context.Games.Where(filter);
                this.Games = result.Count();
                this.Won = result.Count(x => x.Victory);
                this.Lost = result.Count(x => !x.Victory);
                var totalMinutes = result
                    .Select(x => new { x.Started, x.Stopped })
                    .ToList()
                    .Sum(x => x.Stopped.Subtract(x.Started).Minutes);
                this.Hours = totalMinutes / 60;
                this.Minutes = totalMinutes % 60;

                this.TotalGames = context.Games.Count();
                var totaltotalminutes =
                    context.Database.SqlQuery<int?>("SELECT SUM(DATEDIFF(mi, [Started], [Stopped])) FROM GameResults").FirstOrDefault();
                if (totaltotalminutes.HasValue)
                {
                    this.TotalHours = totaltotalminutes.Value / 60;
                    this.TotalMinutes = totaltotalminutes.Value % 60;
                }
            }
        }
    }
}