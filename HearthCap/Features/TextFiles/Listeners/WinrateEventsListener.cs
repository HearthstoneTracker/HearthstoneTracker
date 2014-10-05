// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WinrateEventsListener.cs" company="">
//   
// </copyright>
// <summary>
//   The winrate events listener.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.TextFiles.Listeners
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.GameManager.Events;

    /// <summary>
    /// The winrate events listener.
    /// </summary>
    [Export(typeof(TextFilesEventsListener))]
    public class WinrateEventsListener :
        TextFilesEventsListener, 
        IHandleWithTask<GameResultAdded>, 
        IHandleWithTask<GameResultUpdated>, 
        IHandleWithTask<GameResultDeleted>
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository<GameResult> repository;

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The win rate week.
        /// </summary>
        private const string winRateWeek = "%winrate_week%";

        /// <summary>
        /// The win rate month.
        /// </summary>
        private const string winRateMonth = "%winrate_month%";

        /// <summary>
        /// The win rate this week.
        /// </summary>
        private const string winRateThisWeek = "%winrate_thisweek%";

        /// <summary>
        /// The win rate this month.
        /// </summary>
        private const string winRateThisMonth = "%winrate_thismonth%";

        /// <summary>
        /// Initializes a new instance of the <see cref="WinrateEventsListener"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public WinrateEventsListener(IRepository<GameResult> repository, IEventAggregator events)
        {
            this.repository = repository;
            this.events = events;
            events.Subscribe(this);
            this.Variables.Add(new KeyValuePair<string, string>(winRateWeek, "Current winrate (%) last week (7 days)"));
            this.Variables.Add(new KeyValuePair<string, string>(winRateMonth, "Current winrate (%) last month"));

            // Variables.Add(new KeyValuePair<string, string>(winRateThisWeek, "Winrate (%) this week"));
            // Variables.Add(new KeyValuePair<string, string>(winRateThisMonth, "Winrate (%) this month"));
        }

        /// <summary>
        /// The should handle.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected internal override bool ShouldHandle(string content)
        {
            return content.Contains(winRateMonth) || content.Contains(winRateWeek);
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected internal override string Handle(string content)
        {
            var now = DateTime.Now;
            var lastMonth = now.AddMonths(-1);
            var lastWeek = now.AddDays(-7);

            float totalLastMonth = this.repository.Query(x => x.Count(e => e.Started > lastMonth));
            float winsLastMonth = this.repository.Query(x => x.Count(e => e.Started > lastMonth && e.Victory));
            float totalLastWeek = this.repository.Query(x => x.Count(e => e.Started > lastWeek));
            float winsLastWeek = this.repository.Query(x => x.Count(e => e.Started > lastWeek && e.Victory));
            double winRateLastMonth = Math.Round(winsLastMonth / totalLastMonth * 100, 0);
            double winRateLastWeek = Math.Round(winsLastWeek / totalLastWeek * 100, 0);

            if (content.Contains(winRateWeek))
            {
                content = content.Replace(winRateWeek, winRateLastWeek.ToString(CultureInfo.InvariantCulture));
            }

            if (content.Contains(winRateMonth))
            {
                content = content.Replace(winRateMonth, winRateLastMonth.ToString(CultureInfo.InvariantCulture));
            }

            return content;
        }

        /// <summary>
        /// Handle the message with a Task.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The Task that represents the operation.
        /// </returns>
        public Task Handle(GameResultAdded message)
        {
            return Task.Run(() => this.Refresh());
        }

        /// <summary>
        /// Handle the message with a Task.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The Task that represents the operation.
        /// </returns>
        public Task Handle(GameResultUpdated message)
        {
            return Task.Run(() => this.Refresh());
        }

        /// <summary>
        /// Handle the message with a Task.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The Task that represents the operation.
        /// </returns>
        public Task Handle(GameResultDeleted message)
        {
            return Task.Run(() => this.Refresh());
        }
    }
}