using System.Windows;
using System.Windows.Controls;
using NLog;

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    public class LogTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FatalTemplate { get; set; }
        public DataTemplate ErrorTemplate { get; set; }
        public DataTemplate WarnTemplate { get; set; }
        public DataTemplate InfoTemplate { get; set; }
        public DataTemplate DebugTemplate { get; set; }
        public DataTemplate TraceTemplate { get; set; }

        /// <summary>
        ///     When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate" /> based on custom logic.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="T:System.Windows.DataTemplate" /> or null. The default value is null.
        /// </returns>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var log = item as LogMessageModel;
            if (log == null)
            {
                return null;
            }
            var level = log.Level;

            if (level == LogLevel.Fatal
                && FatalTemplate != null)
            {
                return FatalTemplate;
            }
            if (level == LogLevel.Error
                && ErrorTemplate != null)
            {
                return ErrorTemplate;
            }
            if (level == LogLevel.Warn
                && WarnTemplate != null)
            {
                return WarnTemplate;
            }
            if (level == LogLevel.Info
                && InfoTemplate != null)
            {
                return InfoTemplate;
            }
            if (level == LogLevel.Debug
                && DebugTemplate != null)
            {
                return DebugTemplate;
            }
            if (level == LogLevel.Trace
                && TraceTemplate != null)
            {
                return TraceTemplate;
            }

            return InfoTemplate;
        }
    }
}
