// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaEventsListener.cs" company="">
//   
// </copyright>
// <summary>
//   The arena events listener.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

    /// <summary>
    /// The arena events listener.
    /// </summary>
    [Export(typeof(TextFilesEventsListener))]
    public class ArenaEventsListener :
        TextFilesEventsListener, 
        IHandleWithTask<ArenaSessionUpdated>
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository<ArenaSession> repository;

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The carena wins.
        /// </summary>
        private const string carenaWins = "%carena_wins%";

        /// <summary>
        /// The carena losses.
        /// </summary>
        private const string carenaLosses = "%carena_losses%";

        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaEventsListener"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="events">
        /// The events.
        /// </param>
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
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task Handle(ArenaSessionUpdated message)
        {
            if (message.IsLatest)
            {
                this.Refresh();

                // return Task.Run(
                // () =>
                // {
                // foreach (var tpl in this.Templates)
                // {
                // Handle(tpl, message.ArenaSession);
                // }
                // });
            }

            return null;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="arena">
        /// The arena.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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
            return content.Contains(carenaLosses) || content.Contains(carenaWins);
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="currentContent">
        /// The current content.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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