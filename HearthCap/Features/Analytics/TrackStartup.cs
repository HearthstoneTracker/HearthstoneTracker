namespace HearthCap.Features.Analytics
{
    using System;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.StartUp;

    [Export(typeof(IStartupTask))]
    public class TrackStartup : IStartupTask,
                                IHandle<GameStarted>,
                                IHandle<GameEnded>
    {
        private readonly IEventAggregator events;

        [ImportingConstructor]
        public TrackStartup(IEventAggregator events)
        {
            this.events = events;
            this.events.Subscribe(this);
        }

        public void Run()
        {
            Tracker.TrackEventAsync(Tracker.CommonCategory, "Startup", Tracker.Version.ToString(), 1);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameEnded message)
        {
            var gameLabel = String.Format(
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
        /// <param name="message">The message.</param>
        public void Handle(GameStarted message)
        {
            var gameLabel = String.Format(
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