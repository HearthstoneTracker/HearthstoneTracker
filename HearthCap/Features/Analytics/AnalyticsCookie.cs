namespace HearthCap.Features.Analytics
{
    public class AnalyticsCookie
    {
        public string UniqueVisitorId { get; set; }

        public int FirstVisitTime { get; set; }

        public int PreviousVisitTime { get; set; }

        public int SessionCount { get; set; }
    }
}
