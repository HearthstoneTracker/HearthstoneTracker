using System;
using System.Net;
using GoogleAnalyticsTracker.Core.Interface;

namespace HearthCap.Features.Analytics
{
    public class HsTrackerEnvironment : ITrackerEnvironment
    {
        public string Hostname { get; set; }

        public string OsPlatform { get; set; }

        public string OsVersion { get; set; }

        public string OsVersionString { get; set; }

        public HsTrackerEnvironment()
        {
            Hostname = Dns.GetHostName();
            OsPlatform = Environment.OSVersion.Platform.ToString();
            OsVersion = Environment.OSVersion.Version.ToString();
            OsVersionString = Environment.OSVersion.VersionString;
        }
    }
}
