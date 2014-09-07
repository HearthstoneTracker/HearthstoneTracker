namespace HearthCap.Features.Diagnostics
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.Logging;
    using HearthCap.Core.GameCapture.Logging.LogEvents;

    public class CaptureEngineEventsHandler : PropertyChangedBase,
                                              IHandle<LogEvent>,
                                              IHandle<WindowCaptured>
    {
        private BindableCollection<LogMessageModel> messages;
        private BitmapImage screenshot;

        public CaptureEngineEventsHandler(IEventAggregator events)
        {
            this.messages = new BindableCollection<LogMessageModel>();
            events.Subscribe(this);
        }

        public IObservableCollection<LogMessageModel> Messages
        {
            get
            {
                return this.messages;
            }
        }

        public BitmapImage Screenshot
        {
            get
            {
                return this.screenshot;
            }
            set
            {
                if (Equals(value, this.screenshot)) return;
                this.screenshot = value;
                this.NotifyOfPropertyChange(() => this.Screenshot);
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(LogEvent message)
        {
            Execute.OnUIThread(
                () =>
                    {
                        Messages.Add(message.ToMessageModel());
                    });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(WindowCaptured message)
        {
            Execute.OnUIThread(
                () =>
                    {
                        var ms = new MemoryStream();
                        var bi = new BitmapImage();
                        bi.BeginInit();
                        message.Data.Save(ms, ImageFormat.Bmp);
                        ms.Seek(0, SeekOrigin.Begin);
                        bi.StreamSource = ms;
                        bi.EndInit();
                        this.Screenshot = bi;
                    });
        }
    }
}