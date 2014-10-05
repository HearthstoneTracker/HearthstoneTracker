// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogTemplateSelector.cs" company="">
//   
// </copyright>
// <summary>
//   The log template selector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System.Windows;
    using System.Windows.Controls;

    using NLog;

    /// <summary>
    /// The log template selector.
    /// </summary>
    public class LogTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the fatal template.
        /// </summary>
        public DataTemplate FatalTemplate { get; set; }

        /// <summary>
        /// Gets or sets the error template.
        /// </summary>
        public DataTemplate ErrorTemplate { get; set; }

        /// <summary>
        /// Gets or sets the warn template.
        /// </summary>
        public DataTemplate WarnTemplate { get; set; }

        /// <summary>
        /// Gets or sets the info template.
        /// </summary>
        public DataTemplate InfoTemplate { get; set; }

        /// <summary>
        /// Gets or sets the debug template.
        /// </summary>
        public DataTemplate DebugTemplate { get; set; }

        /// <summary>
        /// Gets or sets the trace template.
        /// </summary>
        public DataTemplate TraceTemplate { get; set; }

        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"/> based on custom logic.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        /// <param name="item">
        /// The data object for which to select the template.
        /// </param>
        /// <param name="container">
        /// The data-bound object.
        /// </param>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var log = item as LogMessageModel;
            if (log == null) return null;
            var level = log.Level;

            if (level == LogLevel.Fatal && this.FatalTemplate != null) return this.FatalTemplate;
            if (level == LogLevel.Error && this.ErrorTemplate != null) return this.ErrorTemplate;
            if (level == LogLevel.Warn && this.WarnTemplate != null) return this.WarnTemplate;
            if (level == LogLevel.Info && this.InfoTemplate != null) return this.InfoTemplate;
            if (level == LogLevel.Debug && this.DebugTemplate != null) return this.DebugTemplate;
            if (level == LogLevel.Trace && this.TraceTemplate != null) return this.TraceTemplate;

            return this.InfoTemplate;
        }
    }
}