// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeManager.cs" company="">
//   
// </copyright>
// <summary>
//   The theme manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Theme
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using HearthCap.Data;
    using HearthCap.StartUp;

    using MahApps.Metro;
    using MahApps.Metro.Controls;

    using NLog;

    /// <summary>
    /// The theme manager.
    /// </summary>
    [Export(typeof(IThemeManager))]
    [Export(typeof(ThemeManager))]
    [Export(typeof(IStartupTask))]
    public class ThemeManager : IThemeManager, IStartupTask
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
        /// The theme resources.
        /// </summary>
        private readonly ResourceDictionary[] themeResources;

        /// <summary>
        /// The current configuration.
        /// </summary>
        private ThemeConfiguration currentConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeManager"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
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

        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        public ThemeConfiguration CurrentConfiguration
        {
            get
            {
                if (this.currentConfiguration == null)
                {
                    this.Run();
                }

                return this.currentConfiguration;
            }
        }

        /// <summary>
        /// Gets or sets the flyout theme.
        /// </summary>
        public FlyoutTheme FlyoutTheme { get; protected set; }

        /// <summary>
        /// The get theme resources.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<ResourceDictionary> GetThemeResources()
        {
            return this.themeResources;
        }

        /// <summary>
        /// The change accent.
        /// </summary>
        /// <param name="accent">
        /// The accent.
        /// </param>
        public void ChangeAccent(Accent accent)
        {
            var theme = MahApps.Metro.ThemeManager.DetectAppStyle(Application.Current);
            MahApps.Metro.ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
            this.CurrentConfiguration.Accent = accent.Name;
            Task.Run(() => this.Save());
        }

        /// <summary>
        /// The apply theme light.
        /// </summary>
        public void ApplyThemeLight()
        {
            var light = MahApps.Metro.ThemeManager.AppThemes.First(x => x.Name.Equals("BaseLight"));
            var theme = MahApps.Metro.ThemeManager.DetectAppStyle(Application.Current);
            MahApps.Metro.ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, light);
            this.CurrentConfiguration.Theme = light.Name;
            Task.Run(() => this.Save());
        }

        /// <summary>
        /// The apply theme dark.
        /// </summary>
        public void ApplyThemeDark()
        {
            var dark = MahApps.Metro.ThemeManager.AppThemes.First(x => x.Name.Equals("BaseDark"));
            var theme = MahApps.Metro.ThemeManager.DetectAppStyle(Application.Current);
            MahApps.Metro.ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, dark);
            this.CurrentConfiguration.Theme = dark.Name;
            Task.Run(() => this.Save());
        }

        /// <summary>
        /// The apply flyout theme.
        /// </summary>
        /// <param name="theme">
        /// The theme.
        /// </param>
        public void ApplyFlyoutTheme(FlyoutTheme theme)
        {
            this.FlyoutTheme = theme;

            using (var reg = new ThemeRegistrySettings())
            {
                reg.FlyoutTheme = theme;
            }

            this.OnFlyoutThemeChanged();
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Save()
        {
            using (var context = this.dbContext())
            {
                var themeconfig = this.CurrentConfiguration;
                context.ThemeConfigurations.Attach(themeconfig);
                context.Entry(themeconfig).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            this.currentConfiguration = this.GetOrCreateThemeConfiguration();

            // MahApps.Metro.ThemeManager.InvalidateSystemResourcesOnBackgroundThread = true;
            this.ApplyConfiguration(this.currentConfiguration);
            using (var reg = new ThemeRegistrySettings())
            {
                this.FlyoutTheme = reg.FlyoutTheme;
            }
        }

        /// <summary>
        /// The apply configuration.
        /// </summary>
        /// <param name="themeConfiguration">
        /// The theme configuration.
        /// </param>
        public void ApplyConfiguration(ThemeConfiguration themeConfiguration = null)
        {
            if (themeConfiguration == null)
            {
                themeConfiguration = this.CurrentConfiguration;
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

        /// <summary>
        /// The get or create theme configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="ThemeConfiguration"/>.
        /// </returns>
        private ThemeConfiguration GetOrCreateThemeConfiguration()
        {
            using (var context = this.dbContext())
            {
                var themeconfig = context.ThemeConfigurations.FirstOrDefault(x => x.Name == "default");
                if (themeconfig == null)
                {
                    var theme = MahApps.Metro.ThemeManager.DetectAppStyle(Application.Current);
                    themeconfig = new ThemeConfiguration { Name = "default", Accent = theme.Item2.Name, Theme = theme.Item1.Name };
                    context.ThemeConfigurations.Add(themeconfig);
                    context.SaveChanges();
                }

                return themeconfig;
            }
        }

        /// <summary>
        /// The flyout theme changed.
        /// </summary>
        public event EventHandler<EventArgs> FlyoutThemeChanged;

        /// <summary>
        /// The on flyout theme changed.
        /// </summary>
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