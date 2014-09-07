//namespace HearthCap.Features.Diagnostics
//{
//    using System;
//    using System.ComponentModel.Composition;

//    using HearthCap.Features.GameCapture.Logging;

//    /// <summary>The capture engine logger.</summary>
//    [Export(typeof(ICaptureEngineLogger))]
//    [Export(typeof(EventCaptureEngineLogger))]
//    public class EventCaptureEngineLogger : CaptureEngineLogger
//    {
//        public EventCaptureEngineLogger()
//            : base()
//        {
//        }

//        public event EventHandler<CaptureEngineLogEventArgs> LogReceived;

//        protected override void Log(string format, LogLevel level, object data, params object[] args)
//        {
//            if (this.LogLevel.HasFlag(level))
//            {
//                this.OnLogReceived(String.Format(format, args), level, DateTime.Now, data);
//            }            
//        }

//        private void OnLogReceived(string message, LogLevel level, DateTime date, object data)
//        {
//            var e = new CaptureEngineLogEventArgs(message, level, date, data);
//            var handler = this.LogReceived;
//            if (handler != null)
//            {
//                handler(this, e);
//            }
//        }
//    }
//}