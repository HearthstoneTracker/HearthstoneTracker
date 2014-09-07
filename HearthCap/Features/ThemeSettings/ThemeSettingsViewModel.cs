namespace HearthCap.Features.ThemeSettings
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Settings;
    using HearthCap.Shell.Theme;

    using MahApps.Metro;
    using MahApps.Metro.Controls;

    using NLog;

    using ThemeManager = MahApps.Metro.ThemeManager;

    [Export(typeof(ISettingsScreen))]
    public class ThemeSettingsViewModel : SettingsScreen
    {
        private static Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly IThemeManager themeManager;

        private BindableCollection<AccentViewModel> accentColors;

        private AccentViewModel activeAccent;

        private bool isThemeLight;

        private bool isThemeDark;

        private bool isFlyoutThemeDark;

        private bool isFlyoutThemeAccent;

        private bool isFlyoutThemeLight;

        [ImportingConstructor]
        public ThemeSettingsViewModel(
            Func<HearthStatsDbContext> dbContext,
            IThemeManager themeManager)
        {
            this.dbContext = dbContext;
            this.themeManager = themeManager;
            this.DisplayName = "Theme settings:";
            this.InitializeTheme();
            this.PropertyChanged += ThemeSettingsViewModel_PropertyChanged;
            Order = 10;
        }

        void ThemeSettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ActiveAccent":
                    ChangeAccent(this.ActiveAccent);
                    break;
            }
        }

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
            this.IsFlyoutThemeAccent = themeManager.FlyoutTheme == FlyoutTheme.Accent;
            this.IsFlyoutThemeLight = themeManager.FlyoutTheme == FlyoutTheme.Light;
            this.IsFlyoutThemeDark = themeManager.FlyoutTheme == FlyoutTheme.Dark;
        }

        public IObservableCollection<AccentViewModel> AccentColors
        {
            get
            {
                return this.accentColors;
            }
        }

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