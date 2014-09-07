namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    using NLog;
    using NLog.Targets;

    public class CaptureTarget : TargetWithLayout
    {
        private static readonly ColorRule DefaultColorRule = new ColorRule();

        public CaptureTarget()
        {
            this.RowColorRules = new List<ColorRule>();
            this.AddDefaultRowColorRules();
        }

        private void AddDefaultRowColorRules()
        {
            var rules = new List<ColorRule>()
                            {
                                new ColorRule("level == LogLevel.Fatal", Brushes.White, Brushes.Red, FontStyles.Normal, FontWeights.Bold),
                                new ColorRule("level == LogLevel.Error", Brushes.Red, Brushes.Transparent, FontStyles.Italic, FontWeights.Bold),
                                new ColorRule("level == LogLevel.Warn", Brushes.Orange, Brushes.Transparent),
                                new ColorRule("level == LogLevel.Info", Brushes.Black, Brushes.Transparent),
                                new ColorRule("level == LogLevel.Debug", Brushes.Gray, Brushes.Transparent),
                                new ColorRule("level == LogLevel.Trace", Brushes.DarkGray, Brushes.Transparent, FontStyles.Italic, FontWeights.Normal)
                            };
            this.RowColorRules.AddRange(rules);
        }

        public List<ColorRule> RowColorRules { get; protected set; }

        public event EventHandler<LogReceivedEventArgs> LogReceived;

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
        /// <param name="logEvent">Logging event to be written out.
        ///             </param>
        protected override void Write(LogEventInfo logEvent)
        {
            var colorrule = this.RowColorRules.FirstOrDefault(x => true.Equals((bool)x.Condition.Evaluate(logEvent)));
            if (colorrule == null)
            {
                colorrule = DefaultColorRule;
            }

            var messageString = GetMessage(logEvent);
            var message = new LogMessageModel(messageString, logEvent.Level, logEvent.TimeStamp)
                              {
                                  FontStyle = colorrule.FontStyle,
                                  FontWeight = colorrule.FontWeight,
                                  BackgroundColor = colorrule.BackgroundColor,
                                  ForegroundColor = colorrule.ForegroundColor
                              };
            this.OnLogReceived(message);
        }

        private string GetMessage(LogEventInfo logEvent)
        {
            return this.Layout.Render(logEvent);
        }
    }
}