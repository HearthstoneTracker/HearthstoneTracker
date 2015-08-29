using System;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;
using HearthCap.Data;

namespace HearthCap.Features.Games.LatestGames
{
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

        public int Games
        {
            get { return games; }
            set
            {
                if (value == games)
                {
                    return;
                }
                games = value;
                NotifyOfPropertyChange(() => Games);
            }
        }

        public int Won
        {
            get { return won; }
            set
            {
                if (value == won)
                {
                    return;
                }
                won = value;
                NotifyOfPropertyChange(() => Won);
            }
        }

        public int Lost
        {
            get { return lost; }
            set
            {
                if (value == lost)
                {
                    return;
                }
                lost = value;
                NotifyOfPropertyChange(() => Lost);
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

        public int TotalGames
        {
            get { return totalGames; }
            set
            {
                if (value == totalGames)
                {
                    return;
                }
                totalGames = value;
                NotifyOfPropertyChange(() => TotalGames);
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

        public void Update(Func<HearthStatsDbContext> dbContext, Expression<Func<GameResult, bool>> filter)
        {
            using (var context = dbContext())
            {
                var result = context.Games.Where(filter);
                Games = result.Count();
                Won = result.Count(x => x.Victory);
                Lost = result.Count(x => !x.Victory);
                var totalMinutes = result
                    .Select(x => new { x.Started, x.Stopped })
                    .ToList()
                    .Sum(x => x.Stopped.Subtract(x.Started).Minutes);
                Hours = totalMinutes / 60;
                Minutes = totalMinutes % 60;

                TotalGames = context.Games.Count();
                var totaltotalminutes =
                    context.Database.SqlQuery<int?>("SELECT SUM(DATEDIFF(mi, [Started], [Stopped])) FROM GameResults").FirstOrDefault();
                if (totaltotalminutes.HasValue)
                {
                    TotalHours = totaltotalminutes.Value / 60;
                    TotalMinutes = totaltotalminutes.Value % 60;
                }
            }
        }
    }
}
