namespace HearthCap.Features.Analytics
{
    using System;
    using GoogleAnalyticsTracker.Core;

    public sealed class HsAnalyticsSession : AnalyticsSession
    {
        protected override string GetUniqueVisitorId()
        {
            var cookie = ParseCookie(Cookie);
            if (String.IsNullOrEmpty(cookie.UniqueVisitorId))
            {
                cookie.UniqueVisitorId = base.GetUniqueVisitorId();
                SaveCookie(cookie);
            }

            return cookie.UniqueVisitorId;
        }

        protected override int GetFirstVisitTime()
        {
            var cookie = ParseCookie(Cookie);
            if (cookie.FirstVisitTime == 0)
            {
                cookie.FirstVisitTime = base.GetFirstVisitTime();
                SaveCookie(cookie);
            }

            return cookie.FirstVisitTime;
        }

        protected override int GetPreviousVisitTime()
        {
            var cookie = ParseCookie(Cookie);
            int previousVisitTime = cookie.PreviousVisitTime;
            cookie.PreviousVisitTime = GetCurrentVisitTime();
            SaveCookie(cookie);
            if (previousVisitTime == 0)
            {
                previousVisitTime = GetCurrentVisitTime();
            }

            return previousVisitTime;
        }

        protected override int GetSessionCount()
        {
            var cookie = ParseCookie(Cookie);
            ++cookie.SessionCount;
            SaveCookie(cookie);
            return cookie.SessionCount;
        }

        private AnalyticsCookie ParseCookie(string cookie)
        {
            var parts = new string[4];
            cookie.Split(new[] { '.' }, 4, StringSplitOptions.RemoveEmptyEntries).CopyTo(parts, 0);
            string uniqueVisitorId = parts[0];
            int firstVisitTime;
            int previousVisitTime;
            int sessionCount;
            int.TryParse(parts[1], out firstVisitTime);
            int.TryParse(parts[2], out previousVisitTime);
            int.TryParse(parts[3], out sessionCount);
            return new AnalyticsCookie()
                       {
                           UniqueVisitorId = uniqueVisitorId,
                           FirstVisitTime = firstVisitTime,
                           PreviousVisitTime = previousVisitTime,
                           SessionCount = sessionCount
                       };
        }

        private void SaveCookie(AnalyticsCookie cookie)
        {
            using (var reg = new AnalyticsRegistrySettings())
            {
                var pstor = string.Format("{0}.{1}.{2}.{3}",
                    cookie.UniqueVisitorId,
                    cookie.FirstVisitTime,
                    cookie.PreviousVisitTime,
                    cookie.SessionCount);
                reg.Cookie = pstor;
            }
        }
    }
}