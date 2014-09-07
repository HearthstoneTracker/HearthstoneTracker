namespace HearthCap.Util
{
    using System;

    public static class DateTimeExtensions
    {
        #region Public Methods and Operators

        public static DateTime SetToBeginOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static DateTime SetToEndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day).AddDays(1).AddTicks(-1);
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
        #endregion
    }
}