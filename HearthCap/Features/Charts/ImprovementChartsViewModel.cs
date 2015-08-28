namespace HearthCap.Features.Charts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Features.Games.Statistics;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    [Export(typeof(IChartTab))]
    public class ImprovementChartsViewModel : ChartTab
    {
        private readonly Func<HearthStatsDbContext> dbContext;

        [ImportingConstructor]
        public ImprovementChartsViewModel(Func<HearthStatsDbContext> dbContext)
        {
            DisplayName = "Over time";
            Order = 1;
            this.dbContext = dbContext;

            PlotModel = new PlotModel()
            {
                Title = "Win ratio",
                IsLegendVisible = true,
            };
            PlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                MinimumRange = 0.50,
                // Minimum = 0,
                MajorStep = 0.05,
                MinorStep = 0.01,
                StringFormat = "P0",
                IsZoomEnabled = false,
                IsPanEnabled = false
            });
            PlotModel.Axes.Add(new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
                Angle = 45,
                IntervalType = DateTimeIntervalType.Auto,
                MinorGridlineStyle = LineStyle.Solid,
                MinorIntervalType = DateTimeIntervalType.Auto,
                StringFormat = "dd MMM",
                IsZoomEnabled = false,
                IsPanEnabled = false
            });
        }

        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public FilteredStatsViewModel GamesStats { get; set; }

        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public ArenaSessions.Statistics.FilteredStatsViewModel ArenaStats { get; set; }

        public PlotModel PlotModel { get; protected set; }

        public override void RefreshData(Expression<Func<GameResult, bool>> gameFilter, Expression<Func<ArenaSession, bool>> arenaFilter)
        {
            PlotModel.Series.Clear();
            using (var context = dbContext())
            {
                var allGames = context.Games
                    .Include("Hero")
                    .Where(gameFilter).ToList();

                // CalculateHeroesWinrate(context, allGames);
                CalculateWinrate(context, allGames);
            }

            PlotModel.InvalidatePlot(true);
        }

        private void CalculateWinrate(HearthStatsDbContext context, List<GameResult> allGames)
        {
            var weekGroups = allGames
                .Select(
                    g => new
                    {
                        Game = g,
                        Year = g.Started.Year,
                        Week =
                             CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(g.Started, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday),
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
                        //Date = g.Key.Date,
                        Wins = g.Sum(x => x.Game.Victory ? 1 : 0),
                        Losses = g.Sum(x => x.Game.Victory ? 0 : 1),
                        Total = g.Count()
                    });
            var dayGroups = allGames
                .Select(
                    g => new
                    {
                        Game = g,
                        Date = g.Started.Date
                    })
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

            PlotModel.Series.Add(
                new LineSeries()
                {
                    Title = "Win ratio per week",
                    ItemsSource = winrateDataPoints,
                    MarkerStroke = OxyColors.Black,
                    MarkerType = MarkerType.Circle,
                    DataFieldX = "Date",
                    DataFieldY = "Value",
                    Smooth = true,
                });

            PlotModel.Series.Add(
                new LineSeries()
                {
                    Title = "Moving average over 7 days",
                    ItemsSource = MovingAverage(winrateAvgDataPoints, 7),
                    MarkerStroke = OxyColors.Black,
                    MarkerType = MarkerType.Circle,
                    DataFieldX = "Date",
                    DataFieldY = "Value",
                    Smooth = true,
                });
        }

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

        private void CalculateHeroesWinrate(HearthStatsDbContext context, List<GameResult> allGames)
        {
            var heroGroups = allGames.GroupBy(x => x.Hero);
            foreach (var heroGroup in heroGroups)
            {
                var weekGroups = heroGroup
                    .Select(
                        g => new
                        {
                            Game = g,
                            Year = g.Started.Year,
                            Week =
                                 CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(g.Started, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday),
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
                            //Date = g.Key.Date,
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
                PlotModel.Series.Add(
                    new LineSeries()
                    {
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

    public class DateValue
    {
        public DateValue(DateTime date, double value)
        {
            this.Date = date;
            this.Value = value;
        }

        public DateTime Date { get; set; }
        public double Value { get; set; }
    }
}