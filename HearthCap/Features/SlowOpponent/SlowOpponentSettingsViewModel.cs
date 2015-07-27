namespace HearthCap.Features.SlowOpponent
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Core.Util;
    using HearthCap.Data;
    using HearthCap.Features.Settings;
    using HearthCap.Shell.Settings;
    using HearthCap.Shell.UserPreferences;

    [Export(typeof(ISettingsScreen))]
    public class SlowOpponentSettingsViewModel : 
        SettingsScreen,
        IHandle<NewRound>
    {
        private readonly IEventAggregator events;

        private bool enableSlowOpponentMode;

        [ImportingConstructor]
        public SlowOpponentSettingsViewModel(IEventAggregator events)
        {
            this.events = events;
            this.DisplayName = "Slow opponent mode:";
            this.Order = 3;
            using (var reg = new CommonSettings())
            {
                EnableSlowOpponentMode = reg.GetOrCreate("EnableSlowOpponentMode", false);
            }

            if (EnableSlowOpponentMode)
            {
                events.Subscribe(this);                
            }
        }

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
                        events.Subscribe(this);
                    }
                    else
                    {
                        events.Unsubscribe(this);
                    }
                }
                this.NotifyOfPropertyChange(() => this.EnableSlowOpponentMode);
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(NewRound message)
        {
            if (EnableSlowOpponentMode && message.MyTurn)
            {
                HearthstoneHelper.SetWindowToForeground();                
            }
        }
    }
}