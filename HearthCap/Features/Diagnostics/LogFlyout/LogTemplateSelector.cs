namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System.Windows;
    using System.Windows.Controls;

    public class LogTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FatalTemplate { get; set; }
        public DataTemplate ErrorTemplate { get; set; }
        public DataTemplate WarnTemplate { get; set; }
        public DataTemplate InfoTemplate { get; set; }
        public DataTemplate DebugTemplate { get; set; }
        public DataTemplate TraceTemplate { get; set; }

        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"/> based on custom logic.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        /// <param name="item">The data object for which to select the template.</param><param name="container">The data-bound object.</param>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var log = item as LogMessageModel;
            if (log == null) return null;
            var level = log.Level;

            if (level == NLog.LogLevel.Fatal && FatalTemplate != null) return FatalTemplate;
            if (level == NLog.LogLevel.Error && ErrorTemplate != null) return ErrorTemplate;
            if (level == NLog.LogLevel.Warn && WarnTemplate != null) return WarnTemplate;
            if (level == NLog.LogLevel.Info && InfoTemplate != null) return InfoTemplate;
            if (level == NLog.LogLevel.Debug && DebugTemplate != null) return DebugTemplate;
            if (level == NLog.LogLevel.Trace && TraceTemplate != null) return TraceTemplate;

            return InfoTemplate;
        }
    }
}