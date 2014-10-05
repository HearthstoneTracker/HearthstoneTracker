// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The about view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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

    /// <summary>
    /// The about view model.
    /// </summary>
    [Export(typeof(IFlyout))]
    public class AboutViewModel : FlyoutViewModel
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public AboutViewModel(IEventAggregator events)
        {
            this.events = events;
            this.Name = "about";
            this.Header = "About";
            this.SetPosition(Position.Left);
            this.CurrentVersion = Assembly.GetEntryAssembly().GetName().Version;
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsOpen" && this.IsOpen)
                {
                    Tracker.TrackEventAsync(Tracker.FlyoutsCategory, "Open", this.Name, 1);
                }
            };
        }

        /// <summary>
        /// Gets or sets the about text.
        /// </summary>
        public string AboutText { get; set; }

        /// <summary>
        /// Gets or sets the license text.
        /// </summary>
        public string LicenseText { get; set; }

        /// <summary>
        /// Gets or sets the current version.
        /// </summary>
        public Version CurrentVersion { get; set; }

        /// <summary>
        /// The visit website.
        /// </summary>
        public void VisitWebsite()
        {
            this.events.PublishOnBackgroundThread(new VisitWebsiteCommand());
        }

        /// <summary>
        /// The donate.
        /// </summary>
        public void Donate()
        {
            this.events.PublishOnBackgroundThread(new VisitWebsiteCommand(Resources.DonationLink));
        }
    }
}