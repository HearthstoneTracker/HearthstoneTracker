// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartStopCommandBarViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The start stop command bar view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.EngineControl
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Core.GameCapture.EngineEvents;
    using HearthCap.Shell.CommandBar;
    using HearthCap.Shell.Dialogs;

    // [Export(typeof(ICommandBarItem))]
    /// <summary>
    /// The start stop command bar view model.
    /// </summary>
    public class StartStopCommandBarViewModel : CommandBarItemViewModel, 
        IHandle<CaptureEngineStarted>, 
        IHandle<CaptureEngineStopped>
    {
        /// <summary>
        /// The dialog manager.
        /// </summary>
        private readonly IDialogManager dialogManager;

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The capture engine.
        /// </summary>
        private readonly ICaptureEngine captureEngine;

        /// <summary>
        /// The is started.
        /// </summary>
        private bool isStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartStopCommandBarViewModel"/> class.
        /// </summary>
        /// <param name="dialogManager">
        /// The dialog manager.
        /// </param>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        /// <param name="captureEngine">
        /// The capture engine.
        /// </param>
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

        /// <summary>
        /// Gets or sets a value indicating whether is started.
        /// </summary>
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
        public void Handle(CaptureEngineStarted message)
        {
            this.IsStarted = true;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(CaptureEngineStopped message)
        {
            this.IsStarted = false;
        }
    }
}