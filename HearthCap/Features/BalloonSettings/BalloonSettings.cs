namespace HearthCap.Features.BalloonSettings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    public static class BalloonTypes
    {
        public const string GameMode = "GameMode";
        public const string GameTurns = "GameTurns";
        public const string GameStartEnd = "GameStartEnd";
    }

    [Export(typeof(BalloonSettings))]
    public class BalloonSettings : PropertyChangedBase
    {
        private bool gameMode;

        private bool gameStartEnd;

        private bool gameTurns;

        private readonly IDictionary<string, Func<BalloonSettings, bool>> balloonTypes = new Dictionary<string, Func<BalloonSettings, bool>>();

        public BalloonSettings()
        {
            this.balloonTypes.Add(BalloonTypes.GameMode, x => x.GameMode);
            this.balloonTypes.Add(BalloonTypes.GameTurns, x => x.GameTurns);
            this.balloonTypes.Add(BalloonTypes.GameStartEnd, x => x.GameStartEnd);

            using (var reg = new BalloonRegistrySettings())
            {
                GameMode = reg.GameMode;
                GameStartEnd = reg.GameStartEnd;
                GameTurns = reg.GameTurns;
            }

            this.PropertyChanged += this.OnPropertyChanged;
        }

        public bool IsEnabled(string balloonType)
        {
            if (balloonTypes.ContainsKey(balloonType))
            {
                return balloonTypes[balloonType](this);
            }
            return false;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            using (var reg = new BalloonRegistrySettings())
            {
                reg.GameMode = this.GameMode;
                reg.GameStartEnd = this.GameStartEnd;
                reg.GameTurns = this.GameTurns;
            }
        }

        public bool GameMode
        {
            get
            {
                return this.gameMode;
            }
            set
            {
                if (value.Equals(this.gameMode)) return;
                this.gameMode = value;
                this.NotifyOfPropertyChange(() => this.GameMode);
            }
        }

        public bool GameStartEnd
        {
            get
            {
                return this.gameStartEnd;
            }
            set
            {
                if (value.Equals(this.gameStartEnd))
                {
                    return;
                }
                this.gameStartEnd = value;
                this.NotifyOfPropertyChange(() => this.GameStartEnd);
            }
        }

        public bool GameTurns
        {
            get
            {
                return this.gameTurns;
            }
            set
            {
                if (value.Equals(this.gameTurns))
                {
                    return;
                }
                this.gameTurns = value;
                this.NotifyOfPropertyChange(() => this.GameTurns);
            }
        }
    }
}