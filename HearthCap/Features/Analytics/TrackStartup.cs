// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackStartup.cs" company="">
//   
// </copyright>
// <summary>
//   The track startup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Analytics
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.StartUp;

    /// <summary>
    /// The track startup.
    /// </summary>
    [Export(typeof(IStartupTask))]
    public class TrackStartup : IStartupTask, 
                                IHandle<GameStarted>, 
                                IHandle<GameEnded>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackStartup"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public TrackStartup(IEventAggregator events)
        {
            this.events = events;
            this.events.Subscribe(this);
        }

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            Tracker.TrackEventAsync(Tracker.CommonCategory, "Startup", Tracker.Version.ToString(), 1);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameEnded message)
        {
            var gameLabel = string.Format(
                "{0}: {1} vs {2}, {3}, {4}", 
                message.GameMode, 
                message.Hero, 
                message.OpponentHero, 
                !message.GoFirst.HasValue ? "?" : (message.GoFirst.Value ? "first" : "second"), 
                !message.Victory.HasValue ? "?" : (message.Victory.Value ? "won" : "lost"));

            Tracker.TrackEventAsync(Tracker.GamesCategory, "GameEnded", gameLabel, 1);
            Tracker.TrackLastPageViewAsync();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameStarted message)
        {
            var gameLabel = string.Format(
                "{0}: {1} vs {2}, {3}", 
                message.GameMode, 
                message.Hero, 
                message.OpponentHero, 
                message.GoFirst ? "first" : "second");

            Tracker.TrackEventAsync(Tracker.GamesCategory, "GameStarted", gameLabel, 1);
            Tracker.TrackPageViewAsync("GameStarted", "GameStarted");
        }
    }
}