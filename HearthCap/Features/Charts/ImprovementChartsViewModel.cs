// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImprovementChartsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The improvement charts view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HearthCap.Features.Charts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Features.Games.Statistics;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    /// <summary>
    /// The improvement charts view model.
    /// </summary>
    [Export(typeof(IChartTab))]
    public class ImprovementChartsViewModel : ChartTab
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImprovementChartsViewModel"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        [ImportingConstructor]
        public ImprovementChartsViewModel(Func<HearthStatsDbContext> dbContext)
        {
            this.DisplayName = "Over time";
            this.Order = 1;
            this.dbContext = dbContext;

            this.PlotModel = new PlotModel("Win ratio")
                            {
                                IsLegendVisible = true, 
                            };
            this.PlotModel.Axes.Add(new LinearAxis(AxisPosition.Left)
                                   {
                                       MinimumRange = 0.50, 
                                       
                                       // Minimum = 0,
                                       MajorStep = 0.05, 
                                       MinorStep = 0.01, 
                                       StringFormat = "P0", 
                                       IsZoomEnabled = false, 
                                       IsPanEnabled = false
                                   });
            this.PlotModel.Axes.Add(new DateTimeAxis(AxisPosition.Bottom)
                                   {
                                       Angle = 45, 
                                       IntervalType = DateTimeIntervalType.Auto, 
                                       MinorGridlineStyle = LineStyle.Solid, 
                                       MinorIntervalType = DateTimeIntervalType.Auto, 
                                       StringFormat = "dd MMM", 
                                       IsZoomEnabled = false, 
                                       IsPanEnabled = false
                                   });
        }

        /// <summary>
        /// Gets or sets the games stats.
        /// </summary>
        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public FilteredStatsViewModel GamesStats { get; set; }

        /// <summary>
        /// Gets or sets the arena stats.
        /// </summary>
        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public ArenaSessions.Statistics.FilteredStatsViewModel ArenaStats { get; set; }

        /// <summary>
        /// Gets or sets the plot model.
        /// </summary>
        public PlotModel PlotModel { get; protected set; }

        /// <summary>
        /// The refresh data.
        /// </summary>
        /// <param name="gameFilter">
        /// The game filter.
        /// </param>
        /// <param name="arenaFilter">
        /// The arena filter.
        /// </param>
        public override void RefreshData(Expression<Func<GameResult, bool>> gameFilter, Expression<Func<ArenaSession, bool>> arenaFilter)
        {
            this.PlotModel.Series.Clear();
            using (var context = this.dbContext())
            {
                var allGames = context.Games
                    .Include("Hero")
                    .Where(gameFilter).ToList();

                // CalculateHeroesWinrate(context, allGames);
                this.CalculateWinrate(context, allGames);
            }

            this.PlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// The calculate winrate.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="allGames">
        /// The all games.
        /// </param>
        private void CalculateWinrate(HearthStatsDbContext context, List<GameResult> allGames)
        {
            var weekGroups = allGames.Select(
                g =>
                new
                    {
                        Game = g, 
                        Year = g.Started.Year, 
                        Week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(g.Started, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday), 
                        Date = g.Started.Date
                    })
                
                // .GroupBy(x => new { x.Date })
                // .OrderBy(x => x.Key.Date)
                .GroupBy(x => new { x.Year, x.Week })
                .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Week)
                .Select(
                    (g, i) => new
                                  {
                                      WeekGroup = g, 
                                      WeekNum = i + 1, 
                                      Year = g.Key.Year, 
                                      CalendarWeek = g.Key.Week, 
                                      
                                      // Date = g.Key.Date,
                                      Wins = g.Sum(x => x.Game.Victory ? 1 : 0), 
                                      Losses = g.Sum(x => x.Game.Victory ? 0 : 1), 
                                      Total = g.Count()
                                  });
            var dayGroups = allGames.Select(g => new { Game = g, Date = g.Started.Date })
                
                // .GroupBy(x => new { x.Date })
                // .OrderBy(x => x.Key.Date)
                .GroupBy(x => x.Date)
                .OrderBy(x => x.Key)
                .Select(
                    (g, i) => new
                    {
                        WeekGroup = g, 
                        Date = g.Key.Date, 
                        Wins = g.Sum(x => x.Game.Victory ? 1 : 0), 
                        Losses = g.Sum(x => x.Game.Victory ? 0 : 1), 
                        Total = g.Count()
                    }); var winrateDataPoints = new List<DateValue>();
            var winrateAvgDataPoints = new List<DateValue>();
            foreach (var weekGroup in weekGroups)
            {
                winrateDataPoints.Add(new DateValue(FirstDateOfWeek(weekGroup.Year, weekGroup.CalendarWeek), weekGroup.Wins / (double)weekGroup.Total));
            }

            foreach (var dayGroup in dayGroups)
            {
                winrateAvgDataPoints.Add(new DateValue(dayGroup.Date, dayGroup.Wins / (double)dayGroup.Total));
            }

            this.PlotModel.Series.Add(
                new LineSeries {
                        Title = "Win ratio per week", 
                        ItemsSource = winrateDataPoints, 
                        MarkerStroke = OxyColors.Black, 
                        MarkerType = MarkerType.Circle, 
                        DataFieldX = "Date", 
                        DataFieldY = "Value", 
                        Smooth = true, 
                    });

            this.PlotModel.Series.Add(
                new LineSeries {
                    Title = "Moving average over 7 days", 
                    ItemsSource = this.MovingAverage(winrateAvgDataPoints, 7), 
                    MarkerStroke = OxyColors.Black, 
                    MarkerType = MarkerType.Circle, 
                    DataFieldX = "Date", 
                    DataFieldY = "Value", 
                    Smooth = true, 
                });
        }

        /// <summary>
        /// The moving average.
        /// </summary>
        /// <param name="series">
        /// The series.
        /// </param>
        /// <param name="period">
        /// The period.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private IEnumerable<DateValue> MovingAverage(List<DateValue> series, int period)
        {
            var result = new List<DateValue>();
            double total = 0;
            for (int i = 0; i < series.Count(); i++)
            {
                if (i >= period)
                {
                    total -= series[i - period].Value;
                }

                total += series[i].Value;
                double average = total / (i >= period ? period : i + 1);
                result.Add(new DateValue(series[i].Date, average));
            }

            return result;
        }

        /// <summary>
        /// The calculate heroes winrate.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="allGames">
        /// The all games.
        /// </param>
        private void CalculateHeroesWinrate(HearthStatsDbContext context, List<GameResult> allGames)
        {
            var heroGroups = allGames.GroupBy(x => x.Hero);
            foreach (var heroGroup in heroGroups)
            {
                var weekGroups = heroGroup.Select(
                    g =>
                    new
                        {
                            Game = g, 
                            Year = g.Started.Year, 
                            Week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(g.Started, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday), 
                            Date = g.Started.Date
                        })
                    
                    // .GroupBy(x => new { x.Date })
                    // .OrderBy(x => x.Key.Date)
                    .GroupBy(x => new { x.Year, x.Week })
                    .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Week)
                    .Select(
                        (g, i) => new
                                      {
                                          WeekGroup = g, 
                                          WeekNum = i + 1, 
                                          Year = g.Key.Year, 
                                          CalendarWeek = g.Key.Week, 
                                          
                                          // Date = g.Key.Date,
                                          Wins = g.Sum(x => x.Game.Victory ? 1 : 0), 
                                          Losses = g.Sum(x => x.Game.Victory ? 0 : 1), 
                                          Total = g.Count()
                                      });
                var winrateDataPoints = new List<DateValue>();
                foreach (var weekGroup in weekGroups)
                {
                    winrateDataPoints.Add(
                        new DateValue(FirstDateOfWeek(weekGroup.Year, weekGroup.CalendarWeek), weekGroup.Wins / (double)weekGroup.Total));

                    // winrateDataPoints.Add(new DateValue(weekGroup.Date, weekGroup.Wins / (double)weekGroup.Total));
                }

                var color = heroGroup.Key.GetColor();
                this.PlotModel.Series.Add(
                    new LineSeries {
                            Title = heroGroup.Key.ClassName, 
                            ItemsSource = winrateDataPoints, 
                            MarkerStroke = OxyColors.Black, 
                            MarkerType = MarkerType.Circle, 
                            DataFieldX = "Date", 
                            DataFieldY = "Value", 
                            Smooth = true, 
                            Color = OxyColor.FromArgb(color.A, color.R, color.G, color.B)
                        });
            }
        }

        /// <summary>
        /// The first date of week.
        /// </summary>
        /// <param name="year">
        /// The year.
        /// </param>
        /// <param name="weekOfYear">
        /// The week of year.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }
    }

    /// <summary>
    /// The date value.
    /// </summary>
    public class DateValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateValue"/> class.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public DateValue(DateTime date, double value)
        {
            this.Date = date;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double Value { get; set; }
    }
}