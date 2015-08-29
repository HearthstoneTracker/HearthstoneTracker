using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using HearthCap.Core.GameCapture.Logging;
using HearthCap.Core.GameCapture.Logging.LogEvents;

namespace HearthCap.Features.Diagnostics
{
    public class CaptureEngineEventsHandler : PropertyChangedBase,
        IHandle<LogEvent>,
        IHandle<WindowCaptured>
    {
        private readonly BindableCollection<LogMessageModel> messages;
        private BitmapImage screenshot;

        public CaptureEngineEventsHandler(IEventAggregator events)
        {
            messages = new BindableCollection<LogMessageModel>();
            events.Subscribe(this);
        }

        public IObservableCollection<LogMessageModel> Messages
        {
            get { return messages; }
        }

        public BitmapImage Screenshot
        {
            get { return screenshot; }
            set
            {
                if (Equals(value, screenshot))
                {
                    return;
                }
                screenshot = value;
                NotifyOfPropertyChange(() => Screenshot);
            }
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(LogEvent message)
        {
            Execute.OnUIThread(
                () => { Messages.Add(message.ToMessageModel()); });
        }

        /// <summary>
        ///     Handles the message.
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
                        Screenshot = bi;
                    });
        }
    }
}
