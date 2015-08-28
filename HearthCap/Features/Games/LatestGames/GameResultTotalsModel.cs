namespace HearthCap.Features.Games.LatestGames
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    using HearthCap.Data;

    public class GameResultTotalsModel : PropertyChangedBase
    {
        private int games;

        private int won;

        private int lost;

        private int hours;

        private int minutes;

        private int totalGames;

        private int totalHours;

        private int totalMinutes;

        public GameResultTotalsModel()
        {
        }

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