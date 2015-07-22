namespace HearthCap.Features.Analytics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using GoogleAnalyticsTracker.Core;
    using GoogleAnalyticsTracker.Core.TrackerParameters;

    using NLog;

    public static class Tracker
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        private static bool isEnabled;

        private static readonly HsTracker instance;

        static Tracker()
        {
            Version = Assembly.GetEntryAssembly().GetName().Version;
            ExtraParameters = new Dictionary<string, string>();
            ScreenResolution = String.Format(
                    "{0}x{1}",
                    System.Windows.SystemParameters.PrimaryScreenWidth,
                    System.Windows.SystemParameters.PrimaryScreenHeight);
            ScreenColors = String.Format("{0}-bit", Screen.PrimaryScreen.BitsPerPixel);

            var osPlatform = Environment.OSVersion.Platform.ToString();
            var osVersion = Environment.OSVersion.Version.ToString();
            var osVersionString = Environment.OSVersion.VersionString;
            UserAgent = String.Format("{0}/{1} ({2}; {3}; {4})", "HearthstoneTracker", Version, osPlatform, osVersion, osVersionString);
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

        public static string ScreenColors { get; set; }

        public static string ScreenResolution { get; set; }

        public const string CommonCategory = "Common";

        public const string GamesCategory = "Games";

        public const string ErrorsCategory = "Errors";

        public const string FlyoutsCategory = "Flyouts";

        public static Dictionary<string, string> ExtraParameters { get; private set; }

        public static string LastPageUrl { get; private set; }

        public static string LastPageTitle { get; private set; }

        public static Version Version { get; private set; }

        public static string UserAgent { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public static HsTracker Instance
        {
            get
            {
                return instance;
            }
        }

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

        public static void TrackPageViewAsync(string pageTitle, string pageUrl)
        {
            if (!IsEnabled) return;
            LastPageTitle = pageTitle;
            LastPageUrl = pageUrl;
            TrackLastPageViewAsync();
        }

        public static void TrackLastPageViewAsync()
        {
            if (!IsEnabled) return;
            var pageView = new PageView();
            FillParameters(pageView);
            Instance.TrackPageViewAsync(pageView);
        }

        public static void TrackEventAsync(string category, string action, string label, int value = 1)
        {
            if (!IsEnabled) return;
            var pageView = new EventTracking()
                {
                    Category = category,
                    Action = action,
                    Label = label,
                    Value = value,
                };
            FillParameters(pageView);
            instance.TrackEventAsync(pageView);
        }

        private static void FillParameters(GeneralParameters parameters)
        {
            parameters.UserAgent = UserAgent;
            parameters.ScreenColors = ScreenColors;
            parameters.ScreenResolution = ScreenResolution;
            parameters.DocumentTitle = LastPageTitle;
            parameters.DocumentLocationUrl = LastPageUrl;
        }
    }
}