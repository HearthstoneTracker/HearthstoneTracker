namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System.Windows;
    using System.Windows.Controls;

    public class LogStyleSelector : StyleSelector
    {
        public Style FatalStyle { get; set; }
        public Style ErrorStyle { get; set; }
        public Style WarnStyle { get; set; }
        public Style InfoStyle { get; set; }
        public Style DebugStyle { get; set; }
        public Style TraceStyle { get; set; }

        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.Style"/> based on custom logic.
        /// </summary>
        /// <returns>
        /// Returns an application-specific style to apply; otherwise, null.
        /// </returns>
        /// <param name="item">The content.</param><param name="container">The element to which the style will be applied.</param>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var log = item as LogMessageModel;
            if (log == null) return null;
            var level = log.Level;

            if (level == NLog.LogLevel.Fatal && this.FatalStyle != null) return this.FatalStyle;
            if (level == NLog.LogLevel.Error && this.ErrorStyle != null) return this.ErrorStyle;
            if (level == NLog.LogLevel.Warn && this.WarnStyle != null) return this.WarnStyle;
            if (level == NLog.LogLevel.Info && this.InfoStyle != null) return this.InfoStyle;
            if (level == NLog.LogLevel.Debug && this.DebugStyle != null) return this.DebugStyle;
            if (level == NLog.LogLevel.Trace && this.TraceStyle != null) return this.TraceStyle;

            return this.InfoStyle;
        }
    }
}