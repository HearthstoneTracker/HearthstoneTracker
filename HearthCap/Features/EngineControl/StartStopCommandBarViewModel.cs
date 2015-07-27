namespace HearthCap.Features.EngineControl
{
    using System;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Shell.CommandBar;

    [Export(typeof(ICommandBarItem))]
    public class StartStopCommandBarViewModel : CommandBarItemViewModel
    {
        private readonly ICaptureEngine _captureEngine;

        private bool _isStarted;

        private bool _isStopping;

        private bool _isStarting;

        private bool _showControls;

        [ImportingConstructor]
        public StartStopCommandBarViewModel(
            IEventAggregator eventAggregator,
            ICaptureEngine captureEngine)
        {
            Order = -2;
            _captureEngine = captureEngine;
            _captureEngine.Started += CaptureEngine_Started;
            _captureEngine.Stopped += CaptureEngine_Stopped;
            eventAggregator.Subscribe(this);
            // lol
            IsStarted = captureEngine.IsRunning;
        }

        private void CaptureEngine_Stopped(object sender, EventArgs e)
        {
            IsStarted = false;
            IsStopping = false;
        }

        private void CaptureEngine_Started(object sender, EventArgs e)
        {
            IsStarted = true;
            IsStarting = false;
        }

        public bool IsStarted
        {
            get
            {
                return _isStarted;
            }
            set
            {
                if (value.Equals(_isStarted))
                {
                    return;
                }
                _isStarted = value;
                NotifyOfPropertyChange(() => IsStarted);
            }
        }

        public bool IsStarting
        {
            get
            {
                return _isStarting;
            }
            set
            {
                if (value.Equals(_isStarting))
                {
                    return;
                }
                _isStarting = value;
                NotifyOfPropertyChange(() => IsStarting);
            }
        }

        public bool IsStopping
        {
            get
            {
                return _isStopping;
            }
            set
            {
                if (value.Equals(_isStopping))
                {
                    return;
                }
                _isStopping = value;
                NotifyOfPropertyChange(() => IsStopping);
            }
        }

        public bool ShowControls
        {
            get
            {
                return _showControls;
            }
            set
            {
                if (value.Equals(_showControls))
                {
                    return;
                }
                _showControls = value;
                NotifyOfPropertyChange(() => ShowControls);
            }
        }

        public void StartEngine()
        {
            IsStarting = true;
            _captureEngine.StartAsync();
        }

        public void StopEngine()
        {
            IsStopping = true;
            _captureEngine.Stop();
        }
    }
}