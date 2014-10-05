// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The date time extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Util
{
    using System;

    /// <summary>
    /// The date time extensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The set to begin of day.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime SetToBeginOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        /// <summary>
        /// The set to end of day.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime SetToEndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day).AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// The start of week.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <param name="startOfWeek">
        /// The start of week.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
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