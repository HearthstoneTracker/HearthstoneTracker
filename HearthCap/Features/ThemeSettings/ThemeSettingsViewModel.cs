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
using LogManager = NLog.LogManager;
using ThemeManager = MahApps.Metro.ThemeManager;

namespace HearthCap.Features.ThemeSettings
{
    [Export(typeof(ISettingsScreen))]
    public class ThemeSettingsViewModel : SettingsScreen
    {
        private static NLog.Logger Log = LogManager.GetCurrentClassLogger();

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
            DisplayName = "Theme settings:";
            InitializeTheme();
            PropertyChanged += ThemeSettingsViewModel_PropertyChanged;
            Order = 10;
        }

        private void ThemeSettingsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ActiveAccent":
                    ChangeAccent(ActiveAccent);
                    break;
            }
        }

        private void InitializeTheme()
        {
            accentColors =
                new BindableCollection<AccentViewModel>(
                    ThemeManager.Accents.Select(a => new AccentViewModel(a.Name, (Brush)a.Resources["AccentColorBrush"])).ToList());

            var currentTheme = themeManager.CurrentConfiguration;
            if (currentTheme != null)
            {
                ActiveAccent = accentColors.First(a => a.Name == currentTheme.Accent);
                IsThemeDark = currentTheme.Theme == "BaseDark";
                IsThemeLight = currentTheme.Theme == "BaseLight";
            }
            IsFlyoutThemeAccent = themeManager.FlyoutTheme == FlyoutTheme.Accent;
            IsFlyoutThemeLight = themeManager.FlyoutTheme == FlyoutTheme.Light;
            IsFlyoutThemeDark = themeManager.FlyoutTheme == FlyoutTheme.Dark;
        }

        public IObservableCollection<AccentViewModel> AccentColors
        {
            get { return accentColors; }
        }

        public AccentViewModel ActiveAccent
        {
            get { return activeAccent; }
            set
            {
                if (Equals(value, activeAccent))
                {
                    return;
                }
                activeAccent = value;
                NotifyOfPropertyChange(() => ActiveAccent);
            }
        }

        public bool IsThemeLight
        {
            get { return isThemeLight; }
            set
            {
                if (value.Equals(isThemeLight))
                {
                    return;
                }

                isThemeLight = value;
                if (value)
                {
                    themeManager.ApplyThemeLight();
                }

                NotifyOfPropertyChange(() => IsThemeLight);
            }
        }

        public bool IsThemeDark
        {
            get { return isThemeDark; }
            set
            {
                if (value.Equals(isThemeDark))
                {
                    return;
                }

                isThemeDark = value;
                if (value)
                {
                    themeManager.ApplyThemeDark();
                }

                NotifyOfPropertyChange(() => IsThemeDark);
            }
        }

        public bool IsFlyoutThemeLight
        {
            get { return isFlyoutThemeLight; }
            set
            {
                if (value.Equals(isFlyoutThemeLight))
                {
                    return;
                }

                isFlyoutThemeLight = value;
                if (value)
                {
                    themeManager.ApplyFlyoutTheme(FlyoutTheme.Light);
                }

                NotifyOfPropertyChange(() => IsFlyoutThemeLight);
            }
        }

        public bool IsFlyoutThemeDark
        {
            get { return isFlyoutThemeDark; }
            set
            {
                if (value.Equals(isFlyoutThemeDark))
                {
                    return;
                }

                isFlyoutThemeDark = value;
                if (value)
                {
                    themeManager.ApplyFlyoutTheme(FlyoutTheme.Dark);
                }

                NotifyOfPropertyChange(() => IsFlyoutThemeDark);
            }
        }

        public bool IsFlyoutThemeAccent
        {
            get { return isFlyoutThemeAccent; }
            set
            {
                if (value.Equals(isFlyoutThemeAccent))
                {
                    return;
                }

                isFlyoutThemeAccent = value;
                if (value)
                {
                    themeManager.ApplyFlyoutTheme(FlyoutTheme.Accent);
                }

                NotifyOfPropertyChange(() => IsFlyoutThemeAccent);
            }
        }

        public void ChangeAccent(AccentViewModel accentVm)
        {
            var accent = ThemeManager.Accents.First(x => x.Name == accentVm.Name);
            themeManager.ChangeAccent(accent);
        }
    }
}
