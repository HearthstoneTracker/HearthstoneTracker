namespace HearthCap.Features.Analytics
{
    using GoogleAnalyticsTracker.Core;

    public class HsTracker
        : TrackerBase
    {
        public HsTracker(string trackingAccount, string trackingDomain)
            : base(trackingAccount, trackingDomain, new HsAnalyticsSession(), new HsTrackerEnvironment())
        {
        }
    }
}