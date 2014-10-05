// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrayIconViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The tray icon view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

    /// <summary>
    /// The tray icon view model.
    /// </summary>
    [Export(typeof(TrayIconViewModel))]
    public class TrayIconViewModel : Screen, 
        IDisposable, 
        IHandle<TrayNotification>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The balloon settings.
        /// </summary>
        private readonly BalloonSettings balloonSettings;

        /// <summary>
        /// The taskbar icon.
        /// </summary>
        private TaskbarIcon taskbarIcon;

        /// <summary>
        /// The is visible.
        /// </summary>
        private bool isVisible;

        /// <summary>
        /// The was visible.
        /// </summary>
        private bool wasVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrayIconViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="balloonSettings">
        /// The balloon settings.
        /// </param>
        [ImportingConstructor]
        public TrayIconViewModel(IEventAggregator events, BalloonSettings balloonSettings)
        {
            this.events = events;
            this.balloonSettings = balloonSettings;
            this.LeftClickCommand = new PublishCommand<TrayIconLeftClick>(events);
            this.DoubleClickCommand = new PublishCommand<TrayIconDoubleClick>(events);
            events.Subscribe(this);
            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible" && this.TaskbarIcon != null)
            {
                this.TaskbarIcon.Visibility = this.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets or sets the left click command.
        /// </summary>
        public ICommand LeftClickCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the double click command.
        /// </summary>
        public ICommand DoubleClickCommand { get; protected set; }

        /// <summary>
        /// Gets the taskbar icon.
        /// </summary>
        public TaskbarIcon TaskbarIcon
        {
            get
            {
                if (this.taskbarIcon == null)
                {
                    var view = this.GetView();
                    if (view is TaskbarIcon)
                    {
                        this.taskbarIcon = (TaskbarIcon)view;
                    }
                    else
                    {
                        this.taskbarIcon = ((UIElement)this.GetView()).FindChildren<TaskbarIcon>().FirstOrDefault();
                    }
                }

                return this.taskbarIcon;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is visible.
        /// </summary>
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

        /// <summary>
        /// Gets the balloon settings.
        /// </summary>
        public BalloonSettings BalloonSettings
        {
            get
            {
                return this.balloonSettings;
            }
        }

        /// <summary>
        /// The quit.
        /// </summary>
        public void Quit()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// The visit website.
        /// </summary>
        public void VisitWebsite()
        {
            this.events.PublishOnBackgroundThread(new VisitWebsiteCommand());
        }

        /// <summary>
        /// The restore window.
        /// </summary>
        public void RestoreWindow()
        {
            this.events.PublishOnBackgroundThread(new RestoreWindowCommand());
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            this.IsVisible = true;
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view">
        /// </param>
        protected override void OnViewLoaded(object view)
        {
            if (this.TaskbarIcon != null)
            {
                this.TaskbarIcon.Visibility = this.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// The show balloon tip.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        public void ShowBalloonTip(string title, string message, object viewModel = null, int timeout = 6000)
        {
            var balloonTipViewModel = new DefaultBalloonTipViewModel {
                                              Title = title, 
                                              Message = message, 
                                              ViewModel = viewModel
                                          };

            this.wasVisible = this.IsVisible;
            if (!this.wasVisible)
            {
                balloonTipViewModel.BalloonClosing += this.BalloonTipViewModelOnBalloonClosing;
                this.IsVisible = true;
            }

            Execute.OnUIThread(
                () =>
                    {
                        var balloonTip = new DefaultBalloonTip { };
                        ViewModelBinder.Bind(balloonTipViewModel, balloonTip, null);
                        this.TaskbarIcon.ShowCustomBalloon(balloonTip, PopupAnimation.Slide, timeout);
                    });
        }

        /// <summary>
        /// The balloon tip view model on balloon closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void BalloonTipViewModelOnBalloonClosing(object sender, EventArgs eventArgs)
        {
            this.IsVisible = this.wasVisible;
            ((DefaultBalloonTipViewModel)sender).BalloonClosing -= this.BalloonTipViewModelOnBalloonClosing;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(TrayNotification message)
        {
            if (!string.IsNullOrEmpty(message.BalloonType))
            {
                if (!this.balloonSettings.IsEnabled(message.BalloonType))
                {
                    return;
                }
            }

            this.ShowBalloonTip(message.Title, message.Message, message.ViewModel, message.Timeout);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.TaskbarIcon.Dispose();
        }
    }
}