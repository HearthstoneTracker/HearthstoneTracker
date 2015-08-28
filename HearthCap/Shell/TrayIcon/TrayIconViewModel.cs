namespace HearthCap.Shell.TrayIcon
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using Caliburn.Micro;

    using Hardcodet.Wpf.TaskbarNotification;

    using HearthCap.Features.BalloonSettings;
    using HearthCap.Shell.Commands;

    using MahApps.Metro.Controls;

    [Export(typeof(TrayIconViewModel))]
    public sealed class TrayIconViewModel : Screen,
        IDisposable,
        IHandle<TrayNotification>
    {
        private readonly IEventAggregator events;

        private readonly BalloonSettings balloonSettings;

        private TaskbarIcon taskbarIcon;

        private bool isVisible;

        private bool wasVisible;

        [ImportingConstructor]
        public TrayIconViewModel(IEventAggregator events, BalloonSettings balloonSettings)
        {
            this.events = events;
            this.balloonSettings = balloonSettings;
            LeftClickCommand = new PublishCommand<TrayIconLeftClick>(events);
            DoubleClickCommand = new PublishCommand<TrayIconDoubleClick>(events);
            events.Subscribe(this);
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible" && TaskbarIcon != null)
            {
                TaskbarIcon.Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ICommand LeftClickCommand { get; private set; }

        public ICommand DoubleClickCommand { get; private set; }

        public TaskbarIcon TaskbarIcon
        {
            get
            {
                if (taskbarIcon == null)
                {
                    var view = GetView();
                    if (view is TaskbarIcon)
                    {
                        taskbarIcon = (TaskbarIcon)view;
                    }
                    else
                    {
                        taskbarIcon = ((UIElement)GetView()).FindChildren<TaskbarIcon>().FirstOrDefault();
                    }
                }
                return taskbarIcon;
            }
        }

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                if (value.Equals(this.isVisible))
                {
                    return;
                }
                this.isVisible = value;
                this.NotifyOfPropertyChange(() => this.IsVisible);
            }
        }

        public BalloonSettings BalloonSettings
        {
            get
            {
                return this.balloonSettings;
            }
        }

        public void Quit()
        {
            Application.Current.Shutdown();
        }

        public void VisitWebsite()
        {
            events.PublishOnBackgroundThread(new VisitWebsiteCommand());
        }

        public void RestoreWindow()
        {
            events.PublishOnBackgroundThread(new RestoreWindowCommand());
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            IsVisible = true;
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view"/>
        protected override void OnViewLoaded(object view)
        {
            if (TaskbarIcon != null)
            {
                TaskbarIcon.Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void ShowBalloonTip(string title, string message, object viewModel = null, int timeout = 6000)
        {
            var balloonTipViewModel = new DefaultBalloonTipViewModel()
                                          {
                                              Title = title,
                                              Message = message,
                                              ViewModel = viewModel
                                          };

            this.wasVisible = this.IsVisible;
            if (!wasVisible)
            {
                balloonTipViewModel.BalloonClosing += BalloonTipViewModelOnBalloonClosing;
                IsVisible = true;
            }

            Execute.OnUIThread(
                () =>
                    {
                        var balloonTip = new DefaultBalloonTip() { };
                        ViewModelBinder.Bind(balloonTipViewModel, balloonTip, null);
                        TaskbarIcon.ShowCustomBalloon(balloonTip, PopupAnimation.Slide, timeout);
                    });
        }

        private void BalloonTipViewModelOnBalloonClosing(object sender, EventArgs eventArgs)
        {
            this.IsVisible = wasVisible;
            ((DefaultBalloonTipViewModel)sender).BalloonClosing -= BalloonTipViewModelOnBalloonClosing;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(TrayNotification message)
        {
            if (!String.IsNullOrEmpty(message.BalloonType))
            {
                if (!balloonSettings.IsEnabled(message.BalloonType))
                {
                    return;
                }
            }

            ShowBalloonTip(message.Title, message.Message, message.ViewModel, message.Timeout);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            taskbarIcon.Dispose();
        }
    }
}