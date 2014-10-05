// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalyticsCookie.cs" company="">
//   
// </copyright>
// <summary>
//   The analytics cookie.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Analytics
{
    /// <summary>
    /// The analytics cookie.
    /// </summary>
    public class AnalyticsCookie
    {
        /// <summary>
        /// Gets or sets the unique visitor id.
        /// </summary>
        public string UniqueVisitorId { get; set; }

        /// <summary>
        /// Gets or sets the first visit time.
        /// </summary>
        public int FirstVisitTime { get; set; }

        /// <summary>
        /// Gets or sets the previous visit time.
        /// </summary>
        public int PreviousVisitTime { get; set; }

        /// <summary>
        /// Gets or sets the session count.
        /// </summary>
        public int SessionCount { get; set; }
    }
}