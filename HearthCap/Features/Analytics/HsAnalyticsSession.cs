namespace HearthCap.Features.Analytics
{
    using System;
    using System.Globalization;

    using GoogleAnalyticsTracker.Core;

    public class HsAnalyticsSession : AnalyticsSession
    {
        private AnalyticsCookie ParseCookie(string cookie)
        {
            var result = new string[4];
            cookie.Split(new[] { '.' }, 4, StringSplitOptions.RemoveEmptyEntries).CopyTo(result, 0);
            return new AnalyticsCookie()
                       {
                           UniqueVisitorId = GetUniqueVisitorId(result[0]),
                           FirstVisitTime = GetFirstVisitTime(result[1]),
                           PreviousVisitTime = GetPreviousVisitTime(result[2]),
                           SessionCount = GetSessionCount(result[3])
                       };
        }

        protected string GetUniqueVisitorId(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                s = string.Format(
                    "{0}{1}",
                    new Random((int)DateTime.UtcNow.Ticks).Next(100000000, 999999999),
                    "00145214523");
            }
            return s;
        }

        protected int GetFirstVisitTime(string s)
        {
            int val;
            if (!int.TryParse(s, out val) || val == 0)
            {
                val = DateTime.UtcNow.ToUnixTime();
            }
            return val;
        }

        protected int GetPreviousVisitTime(string s)
        {
            int val;
            if (!int.TryParse(s, out val) || val == 0)
            {
                val = DateTime.UtcNow.ToUnixTime();
            }
            return val;
        }

        protected int GetSessionCount(string s)
        {
            int val;
            if (!int.TryParse(s, out val) || val == 0)
            {
                val = 1;
            }
            return val;
        }

        public override string GenerateCookieValue()
        {
            AnalyticsCookie cookie;
            using (var reg = new AnalyticsRegistrySettings())
            {
                cookie = ParseCookie(reg.Cookie);
            }

            var currentVisitTime = DateTime.UtcNow.ToUnixTime();

            var cookiestr = string.Format(
                "__utma=1.{0}.{1}.{2}.{3}.{4};+__utmz=1.{3}.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none);",
                cookie.UniqueVisitorId,
                cookie.FirstVisitTime,
                cookie.PreviousVisitTime,
                currentVisitTime,
                cookie.SessionCount);

            cookie.PreviousVisitTime = currentVisitTime;
            SaveCookie(cookie);

            return cookiestr;
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

        public override string GenerateSessionId()
        {
            if (this.SessionId == null)
            {
                this.SessionId = new Random((int)DateTime.UtcNow.Ticks).Next(100000000, 999999999).ToString((IFormatProvider)CultureInfo.InvariantCulture);
                AnalyticsCookie cookie;
                using (var reg = new AnalyticsRegistrySettings())
                {
                    cookie = ParseCookie(reg.Cookie);
                }
                cookie.SessionCount++;
                SaveCookie(cookie);
            }

            return this.SessionId;
        }
    }
}