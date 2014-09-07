namespace HearthCap.Features.EngineControl
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Core.GameCapture.EngineEvents;
    using HearthCap.Shell.CommandBar;
    using HearthCap.Shell.Dialogs;

    // [Export(typeof(ICommandBarItem))]
    public class StartStopCommandBarViewModel : CommandBarItemViewModel,
        IHandle<CaptureEngineStarted>,
        IHandle<CaptureEngineStopped>
    {
        private readonly IDialogManager dialogManager;

        private readonly IEventAggregator events;

        private readonly ICaptureEngine captureEngine;

        private bool isStarted;

        [ImportingConstructor]
        public StartStopCommandBarViewModel(
            IDialogManager dialogManager,
            IEventAggregator eventAggregator,
            ICaptureEngine captureEngine)
        {
            this.Order = -2;
            this.dialogManager = dialogManager;
            this.events = eventAggregator;
            this.captureEngine = captureEngine;
            this.events.Subscribe(this);
            // lol
            this.IsStarted = captureEngine.IsRunning;
        }

        public bool IsStarted
        {
            get
            {
                return this.isStarted;
            }
            set
            {
                if (value.Equals(this.isStarted))
                {
                    return;
                }
                this.isStarted = value;
                this.NotifyOfPropertyChange(() => this.IsStarted);
            }
        }

        public void StartEngine()
        {
            this.captureEngine.StartAsync();
        }

        public void StopEngine()
        {
            this.captureEngine.Stop();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(CaptureEngineStarted message)
        {
            IsStarted = true;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(CaptureEngineStopped message)
        {
            IsStarted = false;
        }
    }
}