// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The startup view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;

    using Caliburn.Micro;

    using HearthCap.Shell.TrayIcon;

    /// <summary>
    /// The startup view model.
    /// </summary>
    [Export(typeof(StartupViewModel))]
    public class StartupViewModel : Screen, 
        IHandle<TrayIconLeftClick>, 
        IHandle<TrayIconDoubleClick>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The window manager.
        /// </summary>
        private readonly CustomWindowManager windowManager;

        /// <summary>
        /// The user preferences.
        /// </summary>
        private readonly UserPreferences.UserPreferences userPreferences;

        /// <summary>
        /// The old height.
        /// </summary>
        private double oldHeight;

        /// <summary>
        /// The old width.
        /// </summary>
        private double oldWidth;

        /// <summary>
        /// The init lock.
        /// </summary>
        private static object initLock = new object();

        /// <summary>
        /// The old left.
        /// </summary>
        private double oldLeft;

        /// <summary>
        /// The old top.
        /// </summary>
        private double oldTop;

        /// <summary>
        /// The shell window.
        /// </summary>
        private Window shellWindow;

        /// <summary>
        /// The tray icon.
        /// </summary>
        private TrayIconViewModel trayIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="trayIcon">
        /// The tray icon.
        /// </param>
        /// <param name="windowManager">
        /// The window manager.
        /// </param>
        /// <param name="userPreferences">
        /// The user preferences.
        /// </param>
        [ImportingConstructor]
        public StartupViewModel(
            IEventAggregator events, 
            IShell shell, 
            TrayIconViewModel trayIcon, 
            CustomWindowManager windowManager, 
            UserPreferences.UserPreferences userPreferences)
        {
            this.events = events;
            this.windowManager = windowManager;
            this.userPreferences = userPreferences;
            this.Shell = shell;
            this.trayIcon = trayIcon;
            events.Subscribe(this);
            this.Shell.Deactivated += this.ShellOnDeactivated;
        }

        /// <summary>
        /// The shell on deactivated.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="deactivationEventArgs">
        /// The deactivation event args.
        /// </param>
        private void ShellOnDeactivated(object sender, DeactivationEventArgs deactivationEventArgs)
        {
            this.TryClose();
        }

        /// <summary>
        /// Gets or sets the shell.
        /// </summary>
        public IShell Shell { get; set; }

        /// <summary>
        /// Gets or sets the tray icon.
        /// </summary>
        public TrayIconViewModel TrayIcon
        {
            get
            {
                return this.trayIcon;
            }

            set
            {
                if (Equals(value, this.trayIcon))
                {
                    return;
                }

                this.trayIcon = value;
                this.NotifyOfPropertyChange(() => this.TrayIcon);
            }
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            lock (initLock)
            {
                ((IActivate)this.TrayIcon).Activate();

                this.oldHeight = this.userPreferences.WindowHeight;
                this.oldWidth = this.userPreferences.WindowWidth;
                this.oldLeft = this.userPreferences.WindowLeft;
                this.oldTop = this.userPreferences.WindowTop;

                this.shellWindow = this.windowManager.EnsureWindow<IShell>();
                var args = Environment.GetCommandLineArgs();
                bool isLogon = args.Any(x => x.Contains("-logon"));
                if (isLogon || this.userPreferences.StartMinimized)
                {
                    if (this.userPreferences.MinimizeToTray)
                    {
                        this.TrayIcon.IsVisible = true;
                        this.shellWindow.ShowInTaskbar = false;
                        this.shellWindow.ShowActivated = false;
                        this.shellWindow.Height = 0;
                        this.shellWindow.Width = 0;
                        this.shellWindow.Show();
                        this.shellWindow.Hide();
                        this.shellWindow.ShowInTaskbar = true;
                        this.shellWindow.ShowActivated = true;
                    }
                    else
                    {
                        this.shellWindow.WindowState = WindowState.Minimized;
                        this.shellWindow.Show();                        
                    }
                }
                else
                {
                    this.Shell.Show();
                }

                this.shellWindow.Height = this.oldHeight;
                this.shellWindow.Width = this.oldWidth;
                this.shellWindow.Left = this.oldLeft;
                this.shellWindow.Top = this.oldTop;
                this.userPreferences.Initialize();
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(TrayIconLeftClick message)
        {
            lock (initLock)
            {
                this.Shell.Show();
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(TrayIconDoubleClick message)
        {
            lock (initLock)
            {
                this.Shell.Show();
            }
        }
    }
}