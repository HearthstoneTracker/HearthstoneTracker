// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureEngineEventsHandler.cs" company="">
//   
// </copyright>
// <summary>
//   The capture engine events handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics
{
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.Logging;
    using HearthCap.Core.GameCapture.Logging.LogEvents;

    /// <summary>
    /// The capture engine events handler.
    /// </summary>
    public class CaptureEngineEventsHandler : PropertyChangedBase, 
                                              IHandle<LogEvent>, 
                                              IHandle<WindowCaptured>
    {
        /// <summary>
        /// The messages.
        /// </summary>
        private BindableCollection<LogMessageModel> messages;

        /// <summary>
        /// The screenshot.
        /// </summary>
        private BitmapImage screenshot;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureEngineEventsHandler"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        public CaptureEngineEventsHandler(IEventAggregator events)
        {
            this.messages = new BindableCollection<LogMessageModel>();
            events.Subscribe(this);
        }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        public IObservableCollection<LogMessageModel> Messages
        {
            get
            {
                return this.messages;
            }
        }

        /// <summary>
        /// Gets or sets the screenshot.
        /// </summary>
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
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(LogEvent message)
        {
            Execute.OnUIThread(
                () =>
                    {
                        this.Messages.Add(message.ToMessageModel());
                    });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
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