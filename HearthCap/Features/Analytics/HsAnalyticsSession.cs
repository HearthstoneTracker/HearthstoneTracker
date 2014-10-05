// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HsAnalyticsSession.cs" company="">
//   
// </copyright>
// <summary>
//   The hs analytics session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Analytics
{
    using System;
    using System.Globalization;

    using GoogleAnalyticsTracker.Core;

    /// <summary>
    /// The hs analytics session.
    /// </summary>
    public class HsAnalyticsSession : IAnalyticsSession
    {
        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// The parse cookie.
        /// </summary>
        /// <param name="cookie">
        /// The cookie.
        /// </param>
        /// <returns>
        /// The <see cref="AnalyticsCookie"/>.
        /// </returns>
        private AnalyticsCookie ParseCookie(string cookie)
        {
            var result = new string[4];
            cookie.Split(new[] { '.' }, 4, StringSplitOptions.RemoveEmptyEntries).CopyTo(result, 0);
            return new AnalyticsCookie {
                           UniqueVisitorId = this.GetUniqueVisitorId(result[0]), 
                           FirstVisitTime = this.GetFirstVisitTime(result[1]), 
                           PreviousVisitTime = this.GetPreviousVisitTime(result[2]), 
                           SessionCount = this.GetSessionCount(result[3])
                       };
        }

        /// <summary>
        /// The get unique visitor id.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetUniqueVisitorId(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                s = string.Format(
                    "{0}{1}", 
                    new Random((int)DateTime.UtcNow.Ticks).Next(100000000, 999999999), 
                    "00145214523");
            }

            return s;
        }

        /// <summary>
        /// The get first visit time.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected int GetFirstVisitTime(string s)
        {
            int val;
            if (!int.TryParse(s, out val) || val == 0)
            {
                val = DateTime.UtcNow.ToUnixTime();
            }

            return val;
        }

        /// <summary>
        /// The get previous visit time.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected int GetPreviousVisitTime(string s)
        {
            int val;
            if (!int.TryParse(s, out val) || val == 0)
            {
                val = DateTime.UtcNow.ToUnixTime();
            }

            return val;
        }

        /// <summary>
        /// The get session count.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected int GetSessionCount(string s)
        {
            int val;
            if (!int.TryParse(s, out val) || val == 0)
            {
                val = 1;
            }

            return val;
        }

        /// <summary>
        /// The generate cookie value.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GenerateCookieValue()
        {
            AnalyticsCookie cookie;
            using (var reg = new AnalyticsRegistrySettings())
            {
                cookie = this.ParseCookie(reg.Cookie);
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
            this.SaveCookie(cookie);

            return cookiestr;
        }

        /// <summary>
        /// The save cookie.
        /// </summary>
        /// <param name="cookie">
        /// The cookie.
        /// </param>
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

        /// <summary>
        /// The generate session id.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GenerateSessionId()
        {
            if (this.SessionId == null)
            {
                this.SessionId = new Random((int)DateTime.UtcNow.Ticks).Next(100000000, 999999999).ToString(CultureInfo.InvariantCulture);
                AnalyticsCookie cookie;
                using (var reg = new AnalyticsRegistrySettings())
                {
                    cookie = this.ParseCookie(reg.Cookie);
                }

                cookie.SessionCount++;
                this.SaveCookie(cookie);
            }

            return this.SessionId;
        }
    }
}