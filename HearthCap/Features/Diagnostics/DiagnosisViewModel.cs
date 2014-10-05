// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagnosisViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The diagnosis view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics
{
    using System;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Core.GameCapture.Logging;
    using HearthCap.Core.GameCapture.Logging.LogEvents;
    using HearthCap.Core.Util;
    using HearthCap.Shell.Tabs;

    // [Export(typeof(ITab))]
    /// <summary>
    /// The diagnosis view model.
    /// </summary>
    public class DiagnosisViewModel : TabViewModel, 
        IHandle<GameEvent>, 
        IHandle<GameModeChanged>, 
        IHandle<DeckDetected>, 
        IHandle<LogEvent>, 
        IHandle<WindowCaptured>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The capture engine.
        /// </summary>
        private readonly ICaptureEngine captureEngine;

        /// <summary>
        /// The screenshot bitmap.
        /// </summary>
        private Bitmap screenshotBitmap;

        /// <summary>
        /// The screenshot.
        /// </summary>
        private BitmapImage screenshot;

        /// <summary>
        /// The notification.
        /// </summary>
        private string notification;

        /// <summary>
        /// The hamming threshold.
        /// </summary>
        private int hammingThreshold = 10;

        /// <summary>
        /// The card distance.
        /// </summary>
        private int cardDistance = 90;

        /// <summary>
        /// The log messages.
        /// </summary>
        private BindableCollection<LogMessageModel> logMessages;

        /// <summary>
        /// The engine events.
        /// </summary>
        private BindableCollection<LogMessageModel> engineEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosisViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="captureEngine">
        /// The capture engine.
        /// </param>
        [ImportingConstructor]
        public DiagnosisViewModel(
            IEventAggregator events, 
            ICaptureEngine captureEngine)
        {
            this.events = events;
            this.captureEngine = captureEngine;
            this.DisplayName = "Diag";
            this.Order = 1000;

            this.engineEvents = new BindableCollection<LogMessageModel>();
            this.logMessages = new BindableCollection<LogMessageModel>();

            // CaptureEngineLogger.Hook(LogAction);
            events.Subscribe(this);
        }

        /// <summary>
        /// The log action.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="logLevel">
        /// The log level.
        /// </param>
        /// <param name="arg3">
        /// The arg 3.
        /// </param>
        private void LogAction(string s, LogLevel logLevel, object arg3)
        {
            if (this.logMessages.Count > 500)
            {
                this.logMessages.Clear();
            }

            // this.logMessages.Add(new LogMessageModel(s, logLevel, DateTime.Now));
        }

        #region Properties

        /// <summary>
        /// Gets or sets the notification.
        /// </summary>
        public string Notification
        {
            get
            {
                return this.notification;
            }

            set
            {
                this.notification = value;
                this.NotifyOfPropertyChange(() => this.Notification);
            }
        }

        /// <summary>
        /// Gets or sets the hamming threshold.
        /// </summary>
        public int HammingThreshold
        {
            get
            {
                return this.hammingThreshold;
            }

            set
            {
                this.hammingThreshold = value;
                this.NotifyOfPropertyChange(() => this.HammingThreshold);
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
                this.screenshot = value;
                this.NotifyOfPropertyChange(() => this.Screenshot);
            }
        }

        /// <summary>
        /// Gets or sets the card distance.
        /// </summary>
        public int CardDistance
        {
            get
            {
                return this.cardDistance;
            }

            set
            {
                this.cardDistance = value;
                this.NotifyOfPropertyChange(() => this.CardDistance);
            }
        }

        /// <summary>
        /// Gets the log messages.
        /// </summary>
        public IObservableCollection<LogMessageModel> LogMessages
        {
            get
            {
                return this.logMessages;
            }
        }

        /// <summary>
        /// Gets the engine events.
        /// </summary>
        public IObservableCollection<LogMessageModel> EngineEvents
        {
            get
            {
                return this.engineEvents;
            }
        }

        #endregion

        /// <summary>
        /// The create screen shot.
        /// </summary>
        public void CreateScreenShot()
        {
            var wnd = HearthstoneHelper.GetHearthstoneWindow();

            if (wnd != IntPtr.Zero)
            {
                var shot = new ScreenCapture();
                this.screenshotBitmap = new Bitmap(shot.GetDesktopBitmap(wnd));
                var ms = new MemoryStream();
                var bi = new BitmapImage();
                bi.BeginInit();
                this.screenshotBitmap.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = ms;
                bi.EndInit();
                this.Screenshot = bi;
            }
            else
            {
                this.Notification = "failed to get window";
            }

            // this.hearthstoneWindowImage.Dispose();                
        }

        /// <summary>
        /// The start recognition.
        /// </summary>
        public void StartRecognition()
        {
            /*
            this.FoundCards.Clear();
            this.Notification = String.Empty;

            if (this.HammingThreshold < 0)
            {
                this.HammingThreshold = 0;
            }

            var path = AppDomain.CurrentDomain.BaseDirectory;
            var hasher = PerceptualHash.Default;
            var cr = new CardRecognizer(new CardHashCache(Path.Combine(path, "hearthstone.json"), Path.Combine(path, "cards"), hasher), hasher);
            cr.HammingThreshold = this.HammingThreshold;
            cr.CardDistance = Convert.ToInt32(this.CardDistance);

            var result = cr.RecognizeWholeWindow(this.screenshotBitmap);
            foreach (var region in result.Regions)
            {
                this.FoundCards.Add(region);
            }

            // Notification.Text = string.Format("Found {0} cards: {1}", cards.Count(), String.Join(", ", cards.Select(c => c.Card.name)));
            // Notification.Text = string.Format("Found 0 cards");            
            */
        }

        /// <summary>
        /// The start engine.
        /// </summary>
        public void StartEngine()
        {
            this.captureEngine.StartAsync();
        }

        /// <summary>
        /// The stop engine.
        /// </summary>
        public void StopEngine()
        {
            this.captureEngine.Stop();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameModeChanged message)
        {
            Execute.OnUIThread(
                () =>
                {
                    if (this.engineEvents.Count > 1000)
                    {
                        this.engineEvents.Clear();
                    }

                    this.EngineEvents.Add(message.ToMessageModel(string.Format("Game mode from '{0}' to '{1}'", message.OldGameMode, message.GameMode)));
                });
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
                    if (!(message is WindowCaptured))
                    {
                        if (this.engineEvents.Count > 1000)
                        {
                            this.engineEvents.Clear();
                        }

                        this.EngineEvents.Add(message.ToMessageModel());
                    }
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
                    // TODO: change 'Screenshot' to Image type and use converter
                    var ms = new MemoryStream();
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    message.Data.Save(ms, ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    this.Screenshot = bi;
                    ms.Dispose();
                });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(DeckDetected message)
        {
            Execute.OnUIThread(
                () =>
                {
                    if (this.engineEvents.Count > 1000)
                    {
                        this.engineEvents.Clear();
                    }

                    this.EngineEvents.Add(message.ToMessageModel(string.Format("Deck detected: {0}", message.Key)));
                });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameEvent message)
        {
            Execute.OnUIThread(
                () =>
                {
                    if (this.engineEvents.Count > 1000)
                    {
                        this.engineEvents.Clear();
                    }

                    this.EngineEvents.Add(message.ToMessageModel("Generic event: " + message.GetType().Name));
                });
        }
    }
}