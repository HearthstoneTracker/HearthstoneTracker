// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The shell view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
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

    /// <summary>
    /// The shell view model.
    /// </summary>
    [Export(typeof(IShell))]
    public class ShellViewModel :
        Conductor<ITab>.Collection.OneActive, 
        IShell, 
        IHandle<ToggleFlyoutCommand>, 
        IHandle<VisitWebsiteCommand>, 
        IHandle<RestoreWindowCommand>
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The dialog manager.
        /// </summary>
        private readonly IDialogManager dialogManager;

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The service locator.
        /// </summary>
        private readonly IServiceLocator serviceLocator;

        /// <summary>
        /// The tabs.
        /// </summary>
        private readonly IEnumerable<ITab> tabs;

        /// <summary>
        /// The update view model.
        /// </summary>
        private readonly UpdateViewModel updateViewModel;

        /// <summary>
        /// The command bar items.
        /// </summary>
        private readonly BindableCollection<ICommandBarItem> commandBarItems;

        /// <summary>
        /// The window commands.
        /// </summary>
        private readonly BindableCollection<IWindowCommand> windowCommands;

        /// <summary>
        /// The flyouts.
        /// </summary>
        private readonly BindableCollection<IFlyout> flyouts;

        /// <summary>
        /// The view ready.
        /// </summary>
        private bool viewReady;

        /// <summary>
        /// The updater.
        /// </summary>
        private UpdateViewModel updater;

        /// <summary>
        /// The update available.
        /// </summary>
        private bool updateAvailable;

        /// <summary>
        /// The window state.
        /// </summary>
        private WindowState windowState;

        /// <summary>
        /// The tray icon.
        /// </summary>
        private TrayIconViewModel trayIcon;

        /// <summary>
        /// The settings manager.
        /// </summary>
        private readonly SettingsManager settingsManager;

        /// <summary>
        /// The support view model.
        /// </summary>
        private readonly SupportViewModel supportViewModel;

        /// <summary>
        /// The user preferences.
        /// </summary>
        private readonly UserPreferences.UserPreferences userPreferences;

        /// <summary>
        /// The was visible.
        /// </summary>
        private bool wasVisible;

        /// <summary>
        /// The servers.
        /// </summary>
        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="dialogManager">
        /// The dialog manager.
        /// </param>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="flyouts">
        /// The flyouts.
        /// </param>
        /// <param name="tabs">
        /// The tabs.
        /// </param>
        /// <param name="windowCommands">
        /// The window commands.
        /// </param>
        /// <param name="commandBarItems">
        /// The command bar items.
        /// </param>
        /// <param name="updateViewModel">
        /// The update view model.
        /// </param>
        /// <param name="userPreferences">
        /// The user preferences.
        /// </param>
        /// <param name="trayIcon">
        /// The tray icon.
        /// </param>
        /// <param name="settingsManager">
        /// The settings manager.
        /// </param>
        /// <param name="supportViewModel">
        /// The support view model.
        /// </param>
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
                        this.WindowState = userPreferences.WindowState;
                    }
                };
            this.WindowState = this.UserPreferences.WindowState;
            this.PropertyChanged += this.ShellViewModel_PropertyChanged;
        }

        /// <summary>
        /// The change server.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        public void ChangeServer(ServerItemModel server)
        {
            this.servers.Default = server;
            server.IsChecked = true;
        }

        /// <summary>
        /// Gets or sets the notifications.
        /// </summary>
        [Import]
        public NotificationsViewModel Notifications { get; set; }

        /// <summary>
        /// Gets or sets the tray icon.
        /// </summary>
        [Import]
        public TrayIconViewModel TrayIcon { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [Import]
        public StatusViewModel Status { get; set; }

        /// <summary>
        /// Gets the servers.
        /// </summary>
        public BindableCollection<ServerItemModel> Servers
        {
            get
            {
                return this.servers;
            }
        }

        /// <summary>
        /// Gets or sets the updater.
        /// </summary>
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

        /// <summary>
        /// Gets the dialogs.
        /// </summary>
        public IDialogManager Dialogs
        {
            get
            {
                return this.dialogManager;
            }
        }

        /// <summary>
        /// Gets the flyouts.
        /// </summary>
        public IObservableCollection<IFlyout> Flyouts
        {
            get
            {
                return this.flyouts;
            }
        }

        /// <summary>
        /// Gets the window commands.
        /// </summary>
        public IObservableCollection<IWindowCommand> WindowCommands
        {
            get
            {
                return this.windowCommands;
            }
        }

        /// <summary>
        /// Gets the command bar items.
        /// </summary>
        public BindableCollection<ICommandBarItem> CommandBarItems
        {
            get
            {
                return this.commandBarItems;
            }
        }

        /// <summary>
        /// The exit to desktop.
        /// </summary>
        public void ExitToDesktop()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// The close.
        /// </summary>
        public void Close()
        {
            this.TryClose();
        }

        #region Flyouts

        /// <summary>
        /// The toggle flyout.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void ToggleFlyout(string name)
        {
            this.ApplyToggleFlyout(name);
        }

        /// <summary>
        /// The toggle flyout.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="isModal">
        /// The is modal.
        /// </param>
        public void ToggleFlyout(string name, bool isModal)
        {
            this.ApplyToggleFlyout(name, null, isModal);
        }

        /// <summary>
        /// The support request.
        /// </summary>
        public void SupportRequest()
        {
            this.dialogManager.ShowDialog(this.supportViewModel);
        }

        /// <summary>
        /// The apply toggle flyout.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="isModal">
        /// The is modal.
        /// </param>
        /// <param name="show">
        /// The show.
        /// </param>
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

        /// <summary>
        /// The visit website.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        public void VisitWebsite(string target)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(target))
                {
                    target = VisitWebsiteCommand.DefaultWebsite;
                }

                Process.Start(target);
            }
            catch (Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                {
                    this.dialogManager.ShowMessageBox(noBrowser.Message, "No browser detected");
                }
            }
            catch (Exception other)
            {
                this.dialogManager.ShowMessageBox(other.Message, "Unknown error");
            }
        }

        /// <summary>
        /// The check for updates.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task CheckForUpdates()
        {
            this.OpenUpdater();
            await this.Updater.CheckForUpdates();
        }

        /// <summary>
        /// The open updater.
        /// </summary>
        public void OpenUpdater()
        {
            this.Updater = this.updateViewModel;
            ((IActivate)this.Updater).Activate();
            this.updateViewModel.Deactivated += (sender, args) =>
            {
                this.Updater = null;
            };
        }

        /// <summary>
        /// The visit website.
        /// </summary>
        public void VisitWebsite()
        {
            this.VisitWebsite(VisitWebsiteCommand.DefaultWebsite);
        }

        /// <summary>
        /// The open data folder.
        /// </summary>
        public void OpenDataFolder()
        {
            var dir = (string)AppDomain.CurrentDomain.GetData("DataDirectory");

            Process.Start("explorer.exe", dir);
        }

        /// <summary>
        /// The change data folder.
        /// </summary>
        public void ChangeDataFolder()
        {
            var current = (string)AppDomain.CurrentDomain.GetData("DataDirectory");

            var dialog = new CommonOpenFileDialog {
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

                Process.Start(Assembly.GetEntryAssembly().Location, "-restarting");
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// The show.
        /// </summary>
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

        /// <summary>
        /// The hide.
        /// </summary>
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
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(VisitWebsiteCommand message)
        {
            var target = message.Website;
            this.VisitWebsite(target);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ToggleFlyoutCommand message)
        {
            this.ApplyToggleFlyout(message.Name, message.IsModal, message.Show);
        }

        /// <summary>
        /// The on activate.
        /// </summary>
        protected override void OnActivate()
        {
        }

        /// <summary>
        /// The on initialize.
        /// </summary>
        protected override async void OnInitialize()
        {
        }

        /// <summary>
        /// Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name="view">
        /// </param>
        protected override void OnViewReady(object view)
        {
            if (this.servers.Default == null)
            {
                var dialog = new ChooseServerDialogViewModel(this.servers);
                dialog.Deactivated += (sender, args) =>
                    {
                        if (args.WasClosed)
                        {
                            this.ChangeServer(dialog.SelectedServer);
                            this.SetMissingServer(dialog.SelectedServer);
                        }
                    };
                this.dialogManager.ShowDialog(dialog);
            }
        }

        /// <summary>
        /// The set missing server.
        /// </summary>
        /// <param name="selectedServer">
        /// The selected server.
        /// </param>
        private void SetMissingServer(ServerItemModel selectedServer)
        {
            if (selectedServer == null) return;
            var serverName = selectedServer.Name;

            using (var context = this.dbContext())
            {
                context.Database.ExecuteSqlCommand("UPDATE GameResults SET Server = @p0 WHERE Server IS NULL OR Server = ''", serverName);
                context.Database.ExecuteSqlCommand("UPDATE ArenaSessions SET Server = @p0 WHERE Server IS NULL OR Server = ''", serverName);
            }

            this.events.PublishOnBackgroundThread(new RefreshAll());
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view">
        /// </param>
        protected override void OnViewLoaded(object view)
        {
            if (!this.viewReady)
            {
                this.viewReady = true;
                Task.Run(
                    async () =>
                    {
                        await this.updateViewModel.CheckForUpdates();
                        this.UpdateAvailable = this.updateViewModel.UpdateAvailable;
                    });

                ((IActivate)this.Notifications).Activate();
                ((IActivate)this.Status).Activate();

                // order tabs
                var ordered = this.tabs.OrderBy(x => x.Order);
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

                    // var activate = flyout as IActivate;
                    // if (activate != null)
                    // {
                    // activate.Activate();
                    // }
                }

                this.events.PublishOnBackgroundThread(new ShellReady());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether update available.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the window state.
        /// </summary>
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
                this.UserPreferences.WindowState = value;
                this.NotifyOfPropertyChange(() => this.WindowState);
                this.events.PublishOnBackgroundThread(new WindowStateChanged(value));
            }
        }

        /// <summary>
        /// Gets the user preferences.
        /// </summary>
        public UserPreferences.UserPreferences UserPreferences
        {
            get
            {
                return this.userPreferences;
            }
        }

        /// <summary>
        /// The shell view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void ShellViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PauseNotify.IsPaused(this)) return;

            switch (e.PropertyName)
            {
                case "WindowState":
                    if (this.WindowState == WindowState.Minimized)
                    {
                        if (!this.userPreferences.MinimizeToTray) return;
                        this.wasVisible = this.trayIcon.IsVisible;
                        this.Hide();
                        this.trayIcon.IsVisible = true;
                        this.trayIcon.ShowBalloonTip("HearthstoneTracker is minimized", "Click the icon to restore it.");
                    }

                    break;
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(RestoreWindowCommand message)
        {
            this.Show();
        }
    }
}