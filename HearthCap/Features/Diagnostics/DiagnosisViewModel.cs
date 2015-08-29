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

namespace HearthCap.Features.Diagnostics
{
    // [Export(typeof(ITab))]
    public sealed class DiagnosisViewModel : TabViewModel,
        IHandle<GameEvent>,
        IHandle<GameModeChanged>,
        IHandle<DeckDetected>,
        IHandle<LogEvent>,
        IHandle<WindowCaptured>,
        IDisposable
    {
        private readonly IEventAggregator events;

        private readonly ICaptureEngine captureEngine;

        private Bitmap screenshotBitmap;

        private BitmapImage screenshot;

        private string notification;

        private int hammingThreshold = 10;

        private int cardDistance = 90;

        private readonly BindableCollection<LogMessageModel> logMessages;

        private readonly BindableCollection<LogMessageModel> engineEvents;

        [ImportingConstructor]
        public DiagnosisViewModel(
            IEventAggregator events,
            ICaptureEngine captureEngine)
        {
            this.events = events;
            this.captureEngine = captureEngine;
            DisplayName = "Diag";
            Order = 1000;

            engineEvents = new BindableCollection<LogMessageModel>();
            logMessages = new BindableCollection<LogMessageModel>();
            // CaptureEngineLogger.Hook(LogAction);
            events.Subscribe(this);
        }

        private void LogAction(string s, LogLevel logLevel, object arg3)
        {
            if (logMessages.Count > 500)
            {
                logMessages.Clear();
            }

            // this.logMessages.Add(new LogMessageModel(s, logLevel, DateTime.Now));
        }

        #region Properties

        public string Notification
        {
            get { return notification; }
            set
            {
                notification = value;
                NotifyOfPropertyChange(() => Notification);
            }
        }

        public int HammingThreshold
        {
            get { return hammingThreshold; }
            set
            {
                hammingThreshold = value;
                NotifyOfPropertyChange(() => HammingThreshold);
            }
        }

        public BitmapImage Screenshot
        {
            get { return screenshot; }
            set
            {
                screenshot = value;
                NotifyOfPropertyChange(() => Screenshot);
            }
        }

        public int CardDistance
        {
            get { return cardDistance; }
            set
            {
                cardDistance = value;
                NotifyOfPropertyChange(() => CardDistance);
            }
        }

        public IObservableCollection<LogMessageModel> LogMessages
        {
            get { return logMessages; }
        }

        public IObservableCollection<LogMessageModel> EngineEvents
        {
            get { return engineEvents; }
        }

        #endregion

        public void CreateScreenShot()
        {
            var wnd = HearthstoneHelper.GetHearthstoneWindow();

            if (wnd != IntPtr.Zero)
            {
                var shot = new ScreenCapture();
                screenshotBitmap = new Bitmap(shot.GetDesktopBitmap(wnd));
                var ms = new MemoryStream();
                var bi = new BitmapImage();
                bi.BeginInit();
                screenshotBitmap.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = ms;
                bi.EndInit();
                Screenshot = bi;
            }
            else
            {
                Notification = "failed to get window";
            }

            // this.hearthstoneWindowImage.Dispose();                
        }

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

        public void StartEngine()
        {
            captureEngine.StartAsync();
        }

        public void StopEngine()
        {
            captureEngine.Stop();
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameModeChanged message)
        {
            Execute.OnUIThread(
                () =>
                    {
                        if (engineEvents.Count > 1000)
                        {
                            engineEvents.Clear();
                        }
                        EngineEvents.Add(message.ToMessageModel(String.Format("Game mode from '{0}' to '{1}'", message.OldGameMode, message.GameMode)));
                    });
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(LogEvent message)
        {
            Execute.OnUIThread(
                () =>
                    {
                        if (engineEvents.Count > 1000)
                        {
                            engineEvents.Clear();
                        }

                        EngineEvents.Add(message.ToMessageModel());
                    });
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
                        // TODO: change 'Screenshot' to Image type and use converter
                        var ms = new MemoryStream();
                        var bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        message.Data.Save(ms, ImageFormat.Bmp);
                        ms.Seek(0, SeekOrigin.Begin);
                        bi.StreamSource = ms;
                        bi.EndInit();
                        Screenshot = bi;
                        ms.Dispose();
                    });
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(DeckDetected message)
        {
            Execute.OnUIThread(
                () =>
                    {
                        if (engineEvents.Count > 1000)
                        {
                            engineEvents.Clear();
                        }

                        EngineEvents.Add(message.ToMessageModel(String.Format("Deck detected: {0}", message.Key)));
                    });
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameEvent message)
        {
            Execute.OnUIThread(
                () =>
                    {
                        if (engineEvents.Count > 1000)
                        {
                            engineEvents.Clear();
                        }

                        EngineEvents.Add(message.ToMessageModel("Generic event: " + message.GetType().Name));
                    });
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (screenshotBitmap != null)
            {
                screenshotBitmap.Dispose();
            }
        }
    }
}
