namespace HearthCap.Features.About
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;

    using Caliburn.Micro;

    using HearthCap.Features.Analytics;
    using HearthCap.Properties;
    using HearthCap.Shell.Commands;
    using HearthCap.Shell.Flyouts;

    using MahApps.Metro.Controls;

    [Export(typeof(IFlyout))]
    public class AboutViewModel : FlyoutViewModel
    {
        private readonly IEventAggregator events;

        [ImportingConstructor]
        public AboutViewModel(IEventAggregator events)
        {
            this.events = events;
            this.Name = "about";
            this.Header = "About";
            SetPosition(Position.Left);
            CurrentVersion = Assembly.GetEntryAssembly().GetName().Version;
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsOpen" && IsOpen)
                {
                    Tracker.TrackEventAsync(Tracker.FlyoutsCategory, "Open", Name, 1);
                }
            };
        }

        public string AboutText { get; set; }

        public string LicenseText { get; set; }

        public Version CurrentVersion { get; set; }

        public void VisitWebsite()
        {
            events.PublishOnBackgroundThread(new VisitWebsiteCommand());
        }

        public void Donate()
        {
            events.PublishOnBackgroundThread(new VisitWebsiteCommand(Resources.DonationLink));
        }
    }
}