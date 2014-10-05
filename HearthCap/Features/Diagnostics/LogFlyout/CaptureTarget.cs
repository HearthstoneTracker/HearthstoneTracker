// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureTarget.cs" company="">
//   
// </copyright>
// <summary>
//   The capture target.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    using NLog;
    using NLog.Targets;

    /// <summary>
    /// The capture target.
    /// </summary>
    public class CaptureTarget : TargetWithLayout
    {
        /// <summary>
        /// The default color rule.
        /// </summary>
        private static readonly ColorRule DefaultColorRule = new ColorRule();

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureTarget"/> class.
        /// </summary>
        public CaptureTarget()
        {
            this.RowColorRules = new List<ColorRule>();
            this.AddDefaultRowColorRules();
        }

        /// <summary>
        /// The add default row color rules.
        /// </summary>
        private void AddDefaultRowColorRules()
        {
            var rules = new List<ColorRule> {
                                new ColorRule("level == LogLevel.Fatal", Brushes.White, Brushes.Red, FontStyles.Normal, FontWeights.Bold), 
                                new ColorRule("level == LogLevel.Error", Brushes.Red, Brushes.Transparent, FontStyles.Italic, FontWeights.Bold), 
                                new ColorRule("level == LogLevel.Warn", Brushes.Orange, Brushes.Transparent), 
                                new ColorRule("level == LogLevel.Info", Brushes.Black, Brushes.Transparent), 
                                new ColorRule("level == LogLevel.Debug", Brushes.Gray, Brushes.Transparent), 
                                new ColorRule("level == LogLevel.Trace", Brushes.DarkGray, Brushes.Transparent, FontStyles.Italic, FontWeights.Normal)
                            };
            this.RowColorRules.AddRange(rules);
        }

        /// <summary>
        /// Gets or sets the row color rules.
        /// </summary>
        public List<ColorRule> RowColorRules { get; protected set; }

        /// <summary>
        /// The log received.
        /// </summary>
        public event EventHandler<LogReceivedEventArgs> LogReceived;

        /// <summary>
        /// The on log received.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        protected virtual void OnLogReceived(LogMessageModel message)
        {
            var e = new LogReceivedEventArgs(message);
            var handler = this.LogReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Writes logging event to the log target.
        ///             classes.
        /// </summary>
        /// <param name="logEvent">
        /// Logging event to be written out.
        ///             </param>
        protected override void Write(LogEventInfo logEvent)
        {
            var colorrule = this.RowColorRules.FirstOrDefault(x => true.Equals((bool)x.Condition.Evaluate(logEvent)));
            if (colorrule == null)
            {
                colorrule = DefaultColorRule;
            }

            var messageString = this.GetMessage(logEvent);
            var message = new LogMessageModel(messageString, logEvent.Level, logEvent.TimeStamp)
                              {
                                  FontStyle = colorrule.FontStyle, 
                                  FontWeight = colorrule.FontWeight, 
                                  BackgroundColor = colorrule.BackgroundColor, 
                                  ForegroundColor = colorrule.ForegroundColor
                              };
            this.OnLogReceived(message);
        }

        /// <summary>
        /// The get message.
        /// </summary>
        /// <param name="logEvent">
        /// The log event.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetMessage(LogEventInfo logEvent)
        {
            return this.Layout.Render(logEvent);
        }
    }
}