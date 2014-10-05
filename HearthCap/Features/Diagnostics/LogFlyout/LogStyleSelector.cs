// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogStyleSelector.cs" company="">
//   
// </copyright>
// <summary>
//   The log style selector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System.Windows;
    using System.Windows.Controls;

    using NLog;

    /// <summary>
    /// The log style selector.
    /// </summary>
    public class LogStyleSelector : StyleSelector
    {
        /// <summary>
        /// Gets or sets the fatal style.
        /// </summary>
        public Style FatalStyle { get; set; }

        /// <summary>
        /// Gets or sets the error style.
        /// </summary>
        public Style ErrorStyle { get; set; }

        /// <summary>
        /// Gets or sets the warn style.
        /// </summary>
        public Style WarnStyle { get; set; }

        /// <summary>
        /// Gets or sets the info style.
        /// </summary>
        public Style InfoStyle { get; set; }

        /// <summary>
        /// Gets or sets the debug style.
        /// </summary>
        public Style DebugStyle { get; set; }

        /// <summary>
        /// Gets or sets the trace style.
        /// </summary>
        public Style TraceStyle { get; set; }

        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.Style"/> based on custom logic.
        /// </summary>
        /// <returns>
        /// Returns an application-specific style to apply; otherwise, null.
        /// </returns>
        /// <param name="item">
        /// The content.
        /// </param>
        /// <param name="container">
        /// The element to which the style will be applied.
        /// </param>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var log = item as LogMessageModel;
            if (log == null) return null;
            var level = log.Level;

            if (level == LogLevel.Fatal && this.FatalStyle != null) return this.FatalStyle;
            if (level == LogLevel.Error && this.ErrorStyle != null) return this.ErrorStyle;
            if (level == LogLevel.Warn && this.WarnStyle != null) return this.WarnStyle;
            if (level == LogLevel.Info && this.InfoStyle != null) return this.InfoStyle;
            if (level == LogLevel.Debug && this.DebugStyle != null) return this.DebugStyle;
            if (level == LogLevel.Trace && this.TraceStyle != null) return this.TraceStyle;

            return this.InfoStyle;
        }
    }
}