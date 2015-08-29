using GoogleAnalyticsTracker.Core;

namespace HearthCap.Features.Analytics
{
    public class HsTracker
        : TrackerBase
    {
        public HsTracker(string trackingAccount, string trackingDomain)
            : base(trackingAccount, trackingDomain, new HsAnalyticsSession(), new HsTrackerEnvironment())
        {
        }
    }
}
