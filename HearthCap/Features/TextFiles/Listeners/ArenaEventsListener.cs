namespace HearthCap.Features.TextFiles.Listeners
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.GameManager.Events;
    using HearthCap.Features.Games.Models;

    [Export(typeof(TextFilesEventsListener))]
    public class ArenaEventsListener :
        TextFilesEventsListener,
        IHandleWithTask<ArenaSessionUpdated>
    {
        private readonly IRepository<ArenaSession> repository;

        private readonly IEventAggregator events;

        private const string carenaWins = "%carena_wins%";
        private const string carenaLosses = "%carena_losses%";

        [ImportingConstructor]
        public ArenaEventsListener(IRepository<ArenaSession> repository, IEventAggregator events)
        {
            this.repository = repository;
            this.events = events;
            events.Subscribe(this);
            this.Variables.Add(new KeyValuePair<string, string>(carenaWins, "Current arena wins"));
            this.Variables.Add(new KeyValuePair<string, string>(carenaLosses, "Current arena losses"));
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public Task Handle(ArenaSessionUpdated message)
        {
            if (message.IsLatest)
            {
                this.Refresh();
                //return Task.Run(
                //    () =>
                //    {
                //        foreach (var tpl in this.Templates)
                //        {
                //            Handle(tpl, message.ArenaSession);
                //        }
                //    });
            }
            return null;
        }

        private string Handle(string content, ArenaSessionModel arena)
        {
            if (content.Contains(carenaWins))
            {
                content = content.Replace(carenaWins, arena.Wins.ToString(CultureInfo.InvariantCulture));
            }
            if (content.Contains(carenaLosses))
            {
                content = content.Replace(carenaLosses, arena.Losses.ToString(CultureInfo.InvariantCulture));
            }
            return content;
        }

        protected internal override bool ShouldHandle(string content)
        {
            return content.Contains(carenaLosses) || content.Contains(carenaWins);
        }

        protected internal override string Handle(string currentContent)
        {
            var latest = this.repository.Query(x => x.OrderByDescending(e => e.StartDate).Take(1).FirstOrDefault());

            if (latest != null)
            {
                return this.Handle(currentContent, latest.ToModel());
            }

            return currentContent;
        }
    }
}