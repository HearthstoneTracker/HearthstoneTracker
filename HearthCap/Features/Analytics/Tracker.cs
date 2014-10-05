// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tracker.cs" company="">
//   
// </copyright>
// <summary>
//   The tracker.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Analytics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;

    using GoogleAnalyticsTracker.Core;

    using NLog;

    /// <summary>
    /// The tracker.
    /// </summary>
    public static class Tracker
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The is enabled.
        /// </summary>
        private static bool isEnabled;

        /// <summary>
        /// The instance.
        /// </summary>
        private static readonly HsTracker instance;

        /// <summary>
        /// Initializes static members of the <see cref="Tracker"/> class.
        /// </summary>
        static Tracker()
        {
            Version = Assembly.GetEntryAssembly().GetName().Version;
            ExtraParameters = new Dictionary<string, string>();
            ExtraParameters[BeaconParameter.Browser.ScreenResolution] = string.Format(
                    "{0}x{1}", 
                    SystemParameters.PrimaryScreenWidth, 
                    SystemParameters.PrimaryScreenHeight);
            ExtraParameters[BeaconParameter.Browser.ScreenColorDepth] = string.Format(
                    "{0}-bit", 
                    Screen.PrimaryScreen.BitsPerPixel);

            var osPlatform = Environment.OSVersion.Platform.ToString();
            var osVersion = Environment.OSVersion.Version.ToString();
            var osVersionString = Environment.OSVersion.VersionString;
            UserAgent = string.Format("{0}/{1} ({2}; {3}; {4})", "HearthstoneTracker", Version, osPlatform, osVersion, osVersionString);
            instance = new HsTracker("UA-46945463-6", "app.hearthstonetracker.com")
            {
                UseSsl = false, 
                UserAgent = UserAgent, 
                ThrowOnErrors = false, 
                Language = CultureInfo.InstalledUICulture.Name
            };

            using (var reg = new AnalyticsRegistrySettings())
            {
                // note: use field here
                isEnabled = reg.ShareUsageStatistics;
            }
        }

        /// <summary>
        /// The common category.
        /// </summary>
        public const string CommonCategory = "Common";

        /// <summary>
        /// The games category.
        /// </summary>
        public const string GamesCategory = "Games";

        /// <summary>
        /// The errors category.
        /// </summary>
        public const string ErrorsCategory = "Errors";

        /// <summary>
        /// The flyouts category.
        /// </summary>
        public const string FlyoutsCategory = "Flyouts";

        /// <summary>
        /// Gets the extra parameters.
        /// </summary>
        public static Dictionary<string, string> ExtraParameters { get; private set; }

        /// <summary>
        /// Gets the last page url.
        /// </summary>
        public static string LastPageUrl { get; private set; }

        /// <summary>
        /// Gets the last page title.
        /// </summary>
        public static string LastPageTitle { get; private set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public static Version Version { get; private set; }

        /// <summary>
        /// Gets the user agent.
        /// </summary>
        public static string UserAgent { get; private set; }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public static HsTracker Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is enabled.
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
                return isEnabled;
            }

            set
            {
                isEnabled = value;
                using (var reg = new AnalyticsRegistrySettings())
                {
                    reg.ShareUsageStatistics = value;
                }
            }
        }

        /// <summary>
        /// The track page view async.
        /// </summary>
        /// <param name="pageTitle">
        /// The page title.
        /// </param>
        /// <param name="pageUrl">
        /// The page url.
        /// </param>
        public static void TrackPageViewAsync(string pageTitle, string pageUrl)
        {
            if (!IsEnabled) return;
            LastPageTitle = pageTitle;
            LastPageUrl = pageUrl;
            Instance.TrackPageViewAsync(pageTitle, pageUrl);
        }

        /// <summary>
        /// The track last page view async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task TrackLastPageViewAsync()
        {
            if (!IsEnabled) return;
            await Instance.TrackPageViewAsync(LastPageTitle, LastPageUrl, ExtraParameters, UserAgent);
        }

        /// <summary>
        /// The track event async.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task TrackEventAsync(string category, string action, string label, int value = 1)
        {
            if (!IsEnabled) return;
            await instance.TrackEventAsync(category, action, label, value, false, ExtraParameters, UserAgent);
        }
    }
}