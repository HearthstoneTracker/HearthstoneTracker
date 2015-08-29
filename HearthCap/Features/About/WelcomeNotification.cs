using System.ComponentModel.Composition;
using Caliburn.Micro;
using HearthCap.Shell.Events;
using HearthCap.Shell.Notifications;
using HearthCap.StartUp;

namespace HearthCap.Features.About
{
    [Export(typeof(IStartupTask))]
    public class WelcomeNotification :
        IStartupTask,
        IHandle<ShellReady>
    {
        private readonly IEventAggregator events;

        [ImportingConstructor]
        public WelcomeNotification(IEventAggregator events)
        {
            this.events = events;
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ShellReady message)
        {
            events.PublishOnBackgroundThread(new SendNotification("Welcome to HeartstoneTracker! Don't forget to visit hearthstonetracker.com for the latest news and updates!", 10000));
        }

        public void Run()
        {
            events.Subscribe(this);
        }
    }
}
