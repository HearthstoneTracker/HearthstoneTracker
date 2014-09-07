namespace HearthCap.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Caliburn.Micro;

    using HearthCap.Composition;
    using HearthCap.Data;
    using HearthCap.Features.AutoUpdate;
    using HearthCap.Features.Core;
    using HearthCap.Features.Servers;
    using HearthCap.Features.Status;
    using HearthCap.Features.Support;
    using HearthCap.Logging;
    using HearthCap.Shell.CommandBar;
    using HearthCap.Shell.Commands;
    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.Events;
    using HearthCap.Shell.Flyouts;
    using HearthCap.Shell.Notifications;
    using HearthCap.Shell.Settings;
    using HearthCap.Shell.Tabs;
    using HearthCap.Shell.TrayIcon;
    using HearthCap.Shell.WindowCommands;
    using HearthCap.Util;

    using Microsoft.WindowsAPICodePack.Dialogs;

    using NLog;

    using LogManager = NLog.LogManager;

    [Export(typeof(IShell))]
    public class ShellViewModel :
        Conductor<ITab>.Collection.OneActive,
        IShell,
        IHandle<ToggleFlyoutCommand>,
        IHandle<VisitWebsiteCommand>,
        IHandle<RestoreWindowCommand>
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IDialogManager dialogManager;

        private readonly IEventAggregator events;

        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly IServiceLocator serviceLocator;

        private readonly IEnumerable<ITab> tabs;

        private readonly UpdateViewModel updateViewModel;

        private readonly BindableCollection<ICommandBarItem> commandBarItems;

        private readonly BindableCollection<IWindowCommand> windowCommands;

        private readonly BindableCollection<IFlyout> flyouts;

        private bool viewReady = false;

        private UpdateViewModel updater;

        private bool updateAvailable;

        private WindowState windowState;

        private TrayIconViewModel trayIcon;

        private readonly SettingsManager settingsManager;

        private readonly SupportViewModel supportViewModel;

        private readonly UserPreferences.UserPreferences userPreferences;

        private bool wasVisible;

        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        [ImportingConstructor]
        public ShellViewModel(
            IServiceLocator serviceLocator,
            IDialogManager dialogManager,
            IEventAggregator events,
            Func<HearthStatsDbContext> dbContext,
            [ImportMany]IEnumerable<IFlyout> flyouts,
            [ImportMany]IEnumerable<ITab> tabs,
            [ImportMany]IEnumerable<IWindowCommand> windowCommands,
            [ImportMany]IEnumerable<ICommandBarItem> commandBarItems,
            UpdateViewModel updateViewModel,
            UserPreferences.UserPreferences userPreferences,
            TrayIconViewModel trayIcon,
            SettingsManager settingsManager,
            SupportViewModel supportViewModel)
        {
            this.dialogManager = dialogManager;
            this.events = events;
            this.dbContext = dbContext;
            this.serviceLocator = serviceLocator;
            this.tabs = tabs;
            this.updateViewModel = updateViewModel;
            this.userPreferences = userPreferences;
            this.trayIcon = trayIcon;
            this.settingsManager = settingsManager;
            this.supportViewModel = supportViewModel;
            this.commandBarItems = new BindableCollection<ICommandBarItem>(commandBarItems.OrderBy(x => x.Order));
            this.windowCommands = new BindableCollection<IWindowCommand>(windowCommands.OrderByDescending(x => x.Order));
            this.flyouts = new BindableCollection<IFlyout>(flyouts);

            this.DisplayName = "HearthstoneTracker.com";
            this.events.Subscribe(this);
            this.userPreferences.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "WindowState")
                    {
                        WindowState = userPreferences.WindowState;
                    }
                };
            this.WindowState = UserPreferences.WindowState;
            this.PropertyChanged += ShellViewModel_PropertyChanged;
        }

        public void ChangeServer(ServerItemModel server)
        {
            servers.Default = server;
            server.IsChecked = true;
        }

        [Import]
        public NotificationsViewModel Notifications { get; set; }

        [Import]
        public TrayIconViewModel TrayIcon { get; set; }

        [Import]
        public StatusViewModel Status { get; set; }

        public BindableCollection<ServerItemModel> Servers
        {
            get
            {
                return servers;
            }
        }

        public UpdateViewModel Updater
        {
            get
            {
                return this.updater;
            }
            set
            {
                if (Equals(value, this.updater))
                {
                    return;
                }
                this.updater = value;
                this.NotifyOfPropertyChange(() => this.Updater);
            }
        }

        public IDialogManager Dialogs
        {
            get
            {
                return dialogManager;
            }
        }

        public IObservableCollection<IFlyout> Flyouts
        {
            get
            {
                return this.flyouts;
            }
        }

        public IObservableCollection<IWindowCommand> WindowCommands
        {
            get
            {
                return this.windowCommands;
            }
        }

        public BindableCollection<ICommandBarItem> CommandBarItems
        {
            get
            {
                return this.commandBarItems;
            }
        }

        public void ExitToDesktop()
        {
            Application.Current.Shutdown();
        }

        public void Close()
        {
            this.TryClose();
        }

        #region Flyouts
        public void ToggleFlyout(string name)
        {
            this.ApplyToggleFlyout(name);
        }

        public void ToggleFlyout(string name, bool isModal)
        {
            this.ApplyToggleFlyout(name, null, isModal);
        }

        public void SupportRequest()
        {
            dialogManager.ShowDialog(supportViewModel);
        }

        protected void ApplyToggleFlyout(string name, bool? isModal = null, bool? show = null)
        {
            Contract.Requires(name != null, "name cannot be null");
            foreach (var f in this.flyouts.Where(x => name.Equals(x.Name)))
            {
                if (isModal.HasValue)
                {
                    f.IsModal = isModal.Value;
                }

                if (show.HasValue)
                {
                    f.IsOpen = show.Value;
                }
                else
                {
                    f.IsOpen = !f.IsOpen;
                }
            }
        }

        #endregion

        public void VisitWebsite(string target)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(target))
                {
                    target = VisitWebsiteCommand.DefaultWebsite;
                }

                System.Diagnostics.Process.Start(target);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                {
                    this.dialogManager.ShowMessageBox(noBrowser.Message, "No browser detected");
                }
            }
            catch (System.Exception other)
            {
                this.dialogManager.ShowMessageBox(other.Message, "Unknown error");
            }
        }

        public async Task CheckForUpdates()
        {
            OpenUpdater();
            await Updater.CheckForUpdates();
        }

        public void OpenUpdater()
        {
            Updater = updateViewModel;
            ((IActivate)Updater).Activate();
            updateViewModel.Deactivated += (sender, args) =>
            {
                Updater = null;
            };
        }

        public void VisitWebsite()
        {
            VisitWebsite(VisitWebsiteCommand.DefaultWebsite);
        }

        public void OpenDataFolder()
        {
            var dir = (string)AppDomain.CurrentDomain.GetData("DataDirectory");

            Process.Start("explorer.exe", dir);
        }

        public void ChangeDataFolder()
        {
            var current = (string)AppDomain.CurrentDomain.GetData("DataDirectory");

            var dialog = new CommonOpenFileDialog()
                             {
                                 InitialDirectory = current,
                                 DefaultDirectory = current,
                                 IsFolderPicker = true,
                                 EnsurePathExists = true
                             };
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                var targetDb = Path.Combine(dialog.FileName, "db.sdf");
                if (!File.Exists(targetDb))
                {
                    var msg = MessageBox.Show("Copy existing database to this location?", "Copy database?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (msg == MessageBoxResult.Yes)
                    {
                        var currentDb = Path.Combine(current, "db.sdf");
                        File.Copy(currentDb, targetDb);
                    }
                }

                using (var reg = new DataDirectorySettings())
                {
                    reg.DataDirectory = dialog.FileName;
                }

                MessageBox.Show(
                    "Application will now restart with new data folder location.",
                    "Restarting",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location, "-restarting");
                Application.Current.Shutdown();
            }
        }

        public void Show()
        {
            Execute.OnUIThread(
                () =>
                {
                    var window = this.GetView() as Window;
                    if (window != null)
                    {
                        window.Show();
                        if (window.WindowState == WindowState.Minimized)
                        {
                            window.WindowState = WindowState.Normal;
                        }
                        window.Activate();
                        window.Topmost = true;  // important
                        window.Topmost = false; // important
                        window.Focus();         // important
                    }
                });
        }

        public void Hide()
        {
            Execute.OnUIThread(
                () =>
                {
                    var window = this.GetView() as Window;
                    if (window != null)
                    {
                        window.Hide();
                    }
                });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(VisitWebsiteCommand message)
        {
            var target = message.Website;
            VisitWebsite(target);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ToggleFlyoutCommand message)
        {
            this.ApplyToggleFlyout(message.Name, message.IsModal, message.Show);
        }

        protected override void OnActivate()
        {
        }

        protected override async void OnInitialize()
        {
        }

        /// <summary>
        /// Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name="view"/>
        protected override void OnViewReady(object view)
        {
            if (servers.Default == null)
            {
                var dialog = new ChooseServerDialogViewModel(servers);
                dialog.Deactivated += (sender, args) =>
                    {
                        if (args.WasClosed)
                        {
                            ChangeServer(dialog.SelectedServer);
                            SetMissingServer(dialog.SelectedServer);
                        }
                    };
                dialogManager.ShowDialog(dialog);
            }
        }

        private void SetMissingServer(ServerItemModel selectedServer)
        {
            if (selectedServer == null) return;
            var serverName = selectedServer.Name;

            using (var context = dbContext())
            {
                context.Database.ExecuteSqlCommand("UPDATE GameResults SET Server = @p0 WHERE Server IS NULL OR Server = ''", serverName);
                context.Database.ExecuteSqlCommand("UPDATE ArenaSessions SET Server = @p0 WHERE Server IS NULL OR Server = ''", serverName);
            }

            events.PublishOnBackgroundThread(new RefreshAll());
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view"/>
        protected override void OnViewLoaded(object view)
        {
            if (!viewReady)
            {
                viewReady = true;
                Task.Run(
                    async () =>
                    {
                        await updateViewModel.CheckForUpdates();
                        UpdateAvailable = updateViewModel.UpdateAvailable;
                    });

                ((IActivate)Notifications).Activate();
                ((IActivate)Status).Activate();

                // order tabs
                var ordered = tabs.OrderBy(x => x.Order);
                foreach (var tab in ordered)
                {
                    this.Items.Add(tab);
                }

                // tabs
                if (this.Items.Count > 0)
                {
                    this.ActivateItem(this.Items.First());
                }

                // flyouts
                foreach (var flyout in this.Flyouts)
                {
                    flyout.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == "IsOpen")
                        {
                            var f = (IFlyout)sender;
                            if (f.IsOpen)
                            {
                                f.Activate();
                            }
                            else
                            {
                                f.Deactivate(false);
                            }
                        }
                    };
                    //var activate = flyout as IActivate;
                    //if (activate != null)
                    //{
                    //    activate.Activate();
                    //}
                }
                this.events.PublishOnBackgroundThread(new ShellReady());
            }
        }

        public bool UpdateAvailable
        {
            get
            {
                return this.updateAvailable;
            }
            set
            {
                if (value.Equals(this.updateAvailable))
                {
                    return;
                }
                this.updateAvailable = value;
                this.NotifyOfPropertyChange(() => this.UpdateAvailable);
            }
        }

        public WindowState WindowState
        {
            get
            {
                return this.windowState;
            }
            set
            {
                if (value == this.windowState)
                {
                    return;
                }
                this.windowState = value;
                UserPreferences.WindowState = value;
                this.NotifyOfPropertyChange(() => this.WindowState);
                events.PublishOnBackgroundThread(new WindowStateChanged(value));
            }
        }

        public UserPreferences.UserPreferences UserPreferences
        {
            get
            {
                return this.userPreferences;
            }
        }

        void ShellViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PauseNotify.IsPaused(this)) return;

            switch (e.PropertyName)
            {
                case "WindowState":
                    if (WindowState == WindowState.Minimized)
                    {
                        if (!userPreferences.MinimizeToTray) return;
                        wasVisible = trayIcon.IsVisible;
                        Hide();
                        trayIcon.IsVisible = true;
                        trayIcon.ShowBalloonTip("HearthstoneTracker is minimized", "Click the icon to restore it.");
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(RestoreWindowCommand message)
        {
            Show();
        }
    }
}