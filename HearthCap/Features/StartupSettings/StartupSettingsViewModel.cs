using System;
using System.ComponentModel.Composition;
using HearthCap.Data;
using HearthCap.Features.Settings;
using HearthCap.Shell.Settings;
using HearthCap.Shell.UserPreferences;

namespace HearthCap.Features.StartupSettings
{
    [Export(typeof(ISettingsScreen))]
    public class StartupSettingsViewModel : SettingsScreen
    {
        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly SettingsManager settingsManager;

        private readonly UserPreferences userPreferences;

        [ImportingConstructor]
        public StartupSettingsViewModel(
            Func<HearthStatsDbContext> dbContext,
            SettingsManager settingsManager,
            UserPreferences userPreferences)
        {
            this.dbContext = dbContext;
            this.settingsManager = settingsManager;
            this.userPreferences = userPreferences;
            DisplayName = "Startup settings:";
            Order = 2;
        }

        public UserPreferences UserPreferences
        {
            get { return userPreferences; }
        }
    }
}
