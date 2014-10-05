// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The theme settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.ThemeSettings
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows.Media;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Settings;
    using HearthCap.Shell.Theme;

    using MahApps.Metro.Controls;

    using NLog;

    using LogManager = NLog.LogManager;
    using ThemeManager = MahApps.Metro.ThemeManager;

    /// <summary>
    /// The theme settings view model.
    /// </summary>
    [Export(typeof(ISettingsScreen))]
    public class ThemeSettingsViewModel : SettingsScreen
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The theme manager.
        /// </summary>
        private readonly IThemeManager themeManager;

        /// <summary>
        /// The accent colors.
        /// </summary>
        private BindableCollection<AccentViewModel> accentColors;

        /// <summary>
        /// The active accent.
        /// </summary>
        private AccentViewModel activeAccent;

        /// <summary>
        /// The is theme light.
        /// </summary>
        private bool isThemeLight;

        /// <summary>
        /// The is theme dark.
        /// </summary>
        private bool isThemeDark;

        /// <summary>
        /// The is flyout theme dark.
        /// </summary>
        private bool isFlyoutThemeDark;

        /// <summary>
        /// The is flyout theme accent.
        /// </summary>
        private bool isFlyoutThemeAccent;

        /// <summary>
        /// The is flyout theme light.
        /// </summary>
        private bool isFlyoutThemeLight;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeSettingsViewModel"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="themeManager">
        /// The theme manager.
        /// </param>
        [ImportingConstructor]
        public ThemeSettingsViewModel(
            Func<HearthStatsDbContext> dbContext, 
            IThemeManager themeManager)
        {
            this.dbContext = dbContext;
            this.themeManager = themeManager;
            this.DisplayName = "Theme settings:";
            this.InitializeTheme();
            this.PropertyChanged += this.ThemeSettingsViewModel_PropertyChanged;
            this.Order = 10;
        }

        /// <summary>
        /// The theme settings view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void ThemeSettingsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ActiveAccent":
                    this.ChangeAccent(this.ActiveAccent);
                    break;
            }
        }

        /// <summary>
        /// The initialize theme.
        /// </summary>
        private void InitializeTheme()
        {
            this.accentColors =
                new BindableCollection<AccentViewModel>(
                    ThemeManager.Accents.Select(a => new AccentViewModel(a.Name, (Brush)a.Resources["AccentColorBrush"])).ToList());

            var currentTheme = this.themeManager.CurrentConfiguration;
            if (currentTheme != null)
            {
                this.ActiveAccent = this.accentColors.First(a => a.Name == currentTheme.Accent);
                this.IsThemeDark = currentTheme.Theme == "BaseDark";
                this.IsThemeLight = currentTheme.Theme == "BaseLight";
            }

            this.IsFlyoutThemeAccent = this.themeManager.FlyoutTheme == FlyoutTheme.Accent;
            this.IsFlyoutThemeLight = this.themeManager.FlyoutTheme == FlyoutTheme.Light;
            this.IsFlyoutThemeDark = this.themeManager.FlyoutTheme == FlyoutTheme.Dark;
        }

        /// <summary>
        /// Gets the accent colors.
        /// </summary>
        public IObservableCollection<AccentViewModel> AccentColors
        {
            get
            {
                return this.accentColors;
            }
        }

        /// <summary>
        /// Gets or sets the active accent.
        /// </summary>
        public AccentViewModel ActiveAccent
        {
            get
            {
                return this.activeAccent;
            }

            set
            {
                if (Equals(value, this.activeAccent))
                {
                    return;
                }

                this.activeAccent = value;
                this.NotifyOfPropertyChange(() => this.ActiveAccent);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is theme light.
        /// </summary>
        public bool IsThemeLight
        {
            get
            {
                return this.isThemeLight;
            }

            set
            {
                if (value.Equals(this.isThemeLight))
                {
                    return;
                }

                this.isThemeLight = value;
                if (value)
                {
                    this.themeManager.ApplyThemeLight();
                }

                this.NotifyOfPropertyChange(() => this.IsThemeLight);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is theme dark.
        /// </summary>
        public bool IsThemeDark
        {
            get
            {
                return this.isThemeDark;
            }

            set
            {
                if (value.Equals(this.isThemeDark))
                {
                    return;
                }

                this.isThemeDark = value;
                if (value)
                {
                    this.themeManager.ApplyThemeDark();
                }

                this.NotifyOfPropertyChange(() => this.IsThemeDark);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is flyout theme light.
        /// </summary>
        public bool IsFlyoutThemeLight
        {
            get
            {
                return this.isFlyoutThemeLight;
            }

            set
            {
                if (value.Equals(this.isFlyoutThemeLight))
                {
                    return;
                }

                this.isFlyoutThemeLight = value;
                if (value)
                {
                    this.themeManager.ApplyFlyoutTheme(FlyoutTheme.Light);
                }

                this.NotifyOfPropertyChange(() => this.IsFlyoutThemeLight);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is flyout theme dark.
        /// </summary>
        public bool IsFlyoutThemeDark
        {
            get
            {
                return this.isFlyoutThemeDark;
            }

            set
            {
                if (value.Equals(this.isFlyoutThemeDark))
                {
                    return;
                }

                this.isFlyoutThemeDark = value;
                if (value)
                {
                    this.themeManager.ApplyFlyoutTheme(FlyoutTheme.Dark);
                }

                this.NotifyOfPropertyChange(() => this.IsFlyoutThemeDark);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is flyout theme accent.
        /// </summary>
        public bool IsFlyoutThemeAccent
        {
            get
            {
                return this.isFlyoutThemeAccent;
            }

            set
            {
                if (value.Equals(this.isFlyoutThemeAccent))
                {
                    return;
                }

                this.isFlyoutThemeAccent = value;
                if (value)
                {
                    this.themeManager.ApplyFlyoutTheme(FlyoutTheme.Accent);
                }

                this.NotifyOfPropertyChange(() => this.IsFlyoutThemeAccent);
            }
        }

        /// <summary>
        /// The change accent.
        /// </summary>
        /// <param name="accentVm">
        /// The accent vm.
        /// </param>
        public void ChangeAccent(AccentViewModel accentVm)
        {
            var accent = ThemeManager.Accents.First(x => x.Name == accentVm.Name);
            this.themeManager.ChangeAccent(accent);
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override async void OnInitialize()
        {
        }
    }
}