namespace HearthCap.Features.Analytics
{
    using System;
    using System.Net;

    using GoogleAnalyticsTracker.Core.Interface;

    public class HsTrackerEnvironment : ITrackerEnvironment
    {
        public string Hostname { get; set; }

        public string OsPlatform { get; set; }

        public string OsVersion { get; set; }

        public string OsVersionString { get; set; }

        public HsTrackerEnvironment()
        {
            this.Hostname = Dns.GetHostName();
            this.OsPlatform = Environment.OSVersion.Platform.ToString();
            this.OsVersion = Environment.OSVersion.Version.ToString();
            this.OsVersionString = Environment.OSVersion.VersionString;
        }
    }
}