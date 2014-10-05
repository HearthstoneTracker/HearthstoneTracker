// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HsTracker.cs" company="">
//   
// </copyright>
// <summary>
//   The hs tracker.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Analytics
{
    using GoogleAnalyticsTracker.Core;

    /// <summary>
    /// The hs tracker.
    /// </summary>
    public class HsTracker
        : TrackerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HsTracker"/> class.
        /// </summary>
        /// <param name="trackingAccount">
        /// The tracking account.
        /// </param>
        /// <param name="trackingDomain">
        /// The tracking domain.
        /// </param>
        public HsTracker(string trackingAccount, string trackingDomain)
            : base(trackingAccount, trackingDomain, new HsAnalyticsSession(), new HsTrackerEnvironment())
        {
        }
    }
}