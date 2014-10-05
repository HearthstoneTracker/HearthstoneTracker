// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WelcomeNotification.cs" company="">
//   
// </copyright>
// <summary>
//   The welcome notification.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.About
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.Events;
    using HearthCap.Shell.Notifications;
    using HearthCap.StartUp;

    /// <summary>
    /// The welcome notification.
    /// </summary>
    [Export(typeof(IStartupTask))]
    public class WelcomeNotification : 
        IStartupTask, 
        IHandle<ShellReady>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeNotification"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public WelcomeNotification(IEventAggregator events)
        {
            this.events = events;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ShellReady message)
        {
            this.events.PublishOnBackgroundThread(new SendNotification("Welcome to HeartstoneTracker! Don't forget to visit hearthstonetracker.com for the latest news and updates!", 10000));
        }

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            this.events.Subscribe(this);
        }
    }
}