using System.Windows;
using System.Windows.Controls;
using NLog;

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    public class LogStyleSelector : StyleSelector
    {
        public Style FatalStyle { get; set; }
        public Style ErrorStyle { get; set; }
        public Style WarnStyle { get; set; }
        public Style InfoStyle { get; set; }
        public Style DebugStyle { get; set; }
        public Style TraceStyle { get; set; }

        /// <summary>
        ///     When overridden in a derived class, returns a <see cref="T:System.Windows.Style" /> based on custom logic.
        /// </summary>
        /// <returns>
        ///     Returns an application-specific style to apply; otherwise, null.
        /// </returns>
        /// <param name="item">The content.</param>
        /// <param name="container">The element to which the style will be applied.</param>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var log = item as LogMessageModel;
            if (log == null)
            {
                return null;
            }
            var level = log.Level;

            if (level == LogLevel.Fatal
                && FatalStyle != null)
            {
                return FatalStyle;
            }
            if (level == LogLevel.Error
                && ErrorStyle != null)
            {
                return ErrorStyle;
            }
            if (level == LogLevel.Warn
                && WarnStyle != null)
            {
                return WarnStyle;
            }
            if (level == LogLevel.Info
                && InfoStyle != null)
            {
                return InfoStyle;
            }
            if (level == LogLevel.Debug
                && DebugStyle != null)
            {
                return DebugStyle;
            }
            if (level == LogLevel.Trace
                && TraceStyle != null)
            {
                return TraceStyle;
            }

            return InfoStyle;
        }
    }
}
