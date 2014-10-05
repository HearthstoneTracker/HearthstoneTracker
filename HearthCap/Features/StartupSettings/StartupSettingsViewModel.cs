// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The startup settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.StartupSettings
{
    using System;
    using System.ComponentModel.Composition;

    using HearthCap.Data;
    using HearthCap.Features.Settings;
    using HearthCap.Shell.Settings;
    using HearthCap.Shell.UserPreferences;

    /// <summary>
    /// The startup settings view model.
    /// </summary>
    [Export(typeof(ISettingsScreen))]
    public class StartupSettingsViewModel : SettingsScreen
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The settings manager.
        /// </summary>
        private readonly SettingsManager settingsManager;

        /// <summary>
        /// The user preferences.
        /// </summary>
        private readonly UserPreferences userPreferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupSettingsViewModel"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="settingsManager">
        /// The settings manager.
        /// </param>
        /// <param name="userPreferences">
        /// The user preferences.
        /// </param>
        [ImportingConstructor]
        public StartupSettingsViewModel(
            Func<HearthStatsDbContext> dbContext, 
            SettingsManager settingsManager, 
            UserPreferences userPreferences)
        {
            this.dbContext = dbContext;
            this.settingsManager = settingsManager;
            this.userPreferences = userPreferences;
            this.DisplayName = "Startup settings:";
            this.Order = 2;
        }

        /// <summary>
        /// Gets the user preferences.
        /// </summary>
        public UserPreferences UserPreferences
        {
            get
            {
                return this.userPreferences;
            }
        }
    }
}