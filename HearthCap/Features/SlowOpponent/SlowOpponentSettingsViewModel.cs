using System.ComponentModel.Composition;
using Caliburn.Micro;
using HearthCap.Core.GameCapture.HS.Events;
using HearthCap.Core.Util;
using HearthCap.Features.Settings;

namespace HearthCap.Features.SlowOpponent
{
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
            DisplayName = "Slow opponent mode:";
            Order = 3;
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
            get { return enableSlowOpponentMode; }
            set
            {
                if (value.Equals(enableSlowOpponentMode))
                {
                    return;
                }
                enableSlowOpponentMode = value;
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
                NotifyOfPropertyChange(() => EnableSlowOpponentMode);
            }
        }

        /// <summary>
        ///     Handles the message.
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
