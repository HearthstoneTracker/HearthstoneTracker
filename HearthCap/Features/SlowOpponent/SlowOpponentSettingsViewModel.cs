// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlowOpponentSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The slow opponent settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.SlowOpponent
{
    using System;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Core.Util;
    using HearthCap.Data;
    using HearthCap.Features.Settings;
    using HearthCap.Shell.Settings;
    using HearthCap.Shell.UserPreferences;

    /// <summary>
    /// The slow opponent settings view model.
    /// </summary>
    [Export(typeof(ISettingsScreen))]
    public class SlowOpponentSettingsViewModel : 
        SettingsScreen, 
        IHandle<NewRound>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

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
        /// The enable slow opponent mode.
        /// </summary>
        private bool enableSlowOpponentMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlowOpponentSettingsViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
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
        public SlowOpponentSettingsViewModel(
            IEventAggregator events, 
            Func<HearthStatsDbContext> dbContext, 
            SettingsManager settingsManager, 
            UserPreferences userPreferences)
        {
            this.events = events;
            this.dbContext = dbContext;
            this.settingsManager = settingsManager;
            this.userPreferences = userPreferences;
            this.DisplayName = "Slow opponent mode:";
            this.Order = 3;
            using (var reg = new CommonSettings())
            {
                this.EnableSlowOpponentMode = reg.GetOrCreate("EnableSlowOpponentMode", false);
            }

            if (this.EnableSlowOpponentMode)
            {
                events.Subscribe(this);                
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable slow opponent mode.
        /// </summary>
        public bool EnableSlowOpponentMode
        {
            get
            {
                return this.enableSlowOpponentMode;
            }

            set
            {
                if (value.Equals(this.enableSlowOpponentMode))
                {
                    return;
                }

                this.enableSlowOpponentMode = value;
                using (var reg = new CommonSettings())
                {
                    reg.SetValue("EnableSlowOpponentMode", value);
                    if (value)
                    {
                        this.events.Subscribe(this);
                    }
                    else
                    {
                        this.events.Unsubscribe(this);
                    }
                }

                this.NotifyOfPropertyChange(() => this.EnableSlowOpponentMode);
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(NewRound message)
        {
            if (this.EnableSlowOpponentMode && message.MyTurn)
            {
                HearthstoneHelper.SetWindowToForeground();                
            }
        }
    }
}