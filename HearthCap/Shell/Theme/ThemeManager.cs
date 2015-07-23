namespace HearthCap.Shell.Theme
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using HearthCap.Data;
    using HearthCap.StartUp;

    using MahApps.Metro;
    using MahApps.Metro.Controls;

    using NLog;

    [Export(typeof(IThemeManager))]
    [Export(typeof(ThemeManager))]
    [Export(typeof(IStartupTask))]
    public class ThemeManager : IThemeManager, IStartupTask
    {
        private static Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly ResourceDictionary[] themeResources;

        private ThemeConfiguration currentConfiguration;

        [ImportingConstructor]
        public ThemeManager(Func<HearthStatsDbContext> dbContext)
        {
            this.dbContext = dbContext;
            this.themeResources = new[]
                                      {
                                          // new ResourceDictionary { Source = new Uri("pack://application:,,,/HearthCap;component/Resources/Icons.xaml") },
                                          new ResourceDictionary { Source = new Uri("/Resources/Theme.xaml", UriKind.Relative) }
                                      };
        }

        public ThemeConfiguration CurrentConfiguration
        {
            get
            {
                if (currentConfiguration == null)
                {
                    Run();
                }
                return this.currentConfiguration;
            }
        }

        public FlyoutTheme FlyoutTheme { get; protected set; }

        public IEnumerable<ResourceDictionary> GetThemeResources()
        {
            return this.themeResources;
        }

        public void ChangeAccent(Accent accent)
        {
            var theme = MahApps.Metro.ThemeManager.DetectAppStyle(Application.Current);
            MahApps.Metro.ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
            CurrentConfiguration.Accent = accent.Name;
            Task.Run(() => Save());
        }

        public void ApplyThemeLight()
        {
            var light = MahApps.Metro.ThemeManager.AppThemes.First(x => x.Name.Equals("BaseLight"));
            var theme = MahApps.Metro.ThemeManager.DetectAppStyle(Application.Current);
            MahApps.Metro.ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, light);
            CurrentConfiguration.Theme = light.Name;
            Task.Run(() => Save());
        }

        public void ApplyThemeDark()
        {
            var dark = MahApps.Metro.ThemeManager.AppThemes.First(x => x.Name.Equals("BaseDark"));
            var theme = MahApps.Metro.ThemeManager.DetectAppStyle(Application.Current);
            MahApps.Metro.ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, dark);
            CurrentConfiguration.Theme = dark.Name;
            Task.Run(() => Save());
        }

        public void ApplyFlyoutTheme(FlyoutTheme theme)
        {
            FlyoutTheme = theme;

            using (var reg = new ThemeRegistrySettings())
            {
                reg.FlyoutTheme = theme;
            }

            OnFlyoutThemeChanged();
        }

        public async Task Save()
        {
            using (var context = dbContext())
            {
                var themeconfig = CurrentConfiguration;
                context.ThemeConfigurations.Attach(themeconfig);
                context.Entry(themeconfig).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public void Run()
        {
            this.currentConfiguration = this.GetOrCreateThemeConfiguration();
            // MahApps.Metro.ThemeManager.InvalidateSystemResourcesOnBackgroundThread = true;
            ApplyConfiguration(this.currentConfiguration);
            using (var reg = new ThemeRegistrySettings())
            {
                FlyoutTheme = reg.FlyoutTheme;
            }
        }

        public void ApplyConfiguration(ThemeConfiguration themeConfiguration = null)
        {
            if (themeConfiguration == null)
            {
                themeConfiguration = CurrentConfiguration;
            }

            var currenttheme = MahApps.Metro.ThemeManager.DetectAppStyle(Application.Current);
            var theme = MahApps.Metro.ThemeManager.AppThemes.FirstOrDefault(x => x.Name == themeConfiguration.Theme);
            if (theme == null)
            {
                theme = currenttheme.Item1;
            }
            var accent = MahApps.Metro.ThemeManager.Accents.FirstOrDefault(x => x.Name == themeConfiguration.Accent);
            if (accent == null)
            {
                accent = currenttheme.Item2;
            }

            MahApps.Metro.ThemeManager.ChangeAppStyle(Application.Current, accent, theme);
        }

        private ThemeConfiguration GetOrCreateThemeConfiguration()
        {
            using (var context = dbContext())
            {
                var themeconfig = context.ThemeConfigurations.FirstOrDefault(x => x.Name == "default");
                if (themeconfig == null)
                {
                    var theme = MahApps.Metro.ThemeManager.DetectAppStyle(Application.Current);
                    themeconfig = new ThemeConfiguration() { Name = "default", Accent = theme.Item2.Name, Theme = theme.Item1.Name };
                    context.ThemeConfigurations.Add(themeconfig);
                    context.SaveChanges();
                }

                return themeconfig;
            }
        }

        public event EventHandler<EventArgs> FlyoutThemeChanged;

        protected virtual void OnFlyoutThemeChanged()
        {
            var handler = this.FlyoutThemeChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}