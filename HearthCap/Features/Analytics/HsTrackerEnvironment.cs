// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HsTrackerEnvironment.cs" company="">
//   
// </copyright>
// <summary>
//   The hs tracker environment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Analytics
{
    using System;
    using System.Net;

    using GoogleAnalyticsTracker.Core;

    /// <summary>
    /// The hs tracker environment.
    /// </summary>
    public class HsTrackerEnvironment : ITrackerEnvironment
    {
        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the os platform.
        /// </summary>
        public string OsPlatform { get; set; }

        /// <summary>
        /// Gets or sets the os version.
        /// </summary>
        public string OsVersion { get; set; }

        /// <summary>
        /// Gets or sets the os version string.
        /// </summary>
        public string OsVersionString { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HsTrackerEnvironment"/> class.
        /// </summary>
        public HsTrackerEnvironment()
        {
            this.Hostname = Dns.GetHostName();
            this.OsPlatform = Environment.OSVersion.Platform.ToString();
            this.OsVersion = Environment.OSVersion.Version.ToString();
            this.OsVersionString = Environment.OSVersion.VersionString;
        }
    }
}