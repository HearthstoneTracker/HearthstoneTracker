using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace HearthCap.Features.BalloonSettings
{
    public static class BalloonTypes
    {
        public const string GameMode = "GameMode";
        public const string GameTurns = "GameTurns";
        public const string GameStartEnd = "GameStartEnd";
    }

    [Export(typeof(BalloonSettings))]
    public class BalloonSettings : PropertyChangedBase
    {
        private bool _gameMode;

        private bool _gameStartEnd;

        private bool _gameTurns;

        private readonly IDictionary<string, Func<BalloonSettings, bool>> _balloonTypes = new Dictionary<string, Func<BalloonSettings, bool>>();

        public BalloonSettings()
        {
            _balloonTypes.Add(BalloonTypes.GameMode, x => x.GameMode);
            _balloonTypes.Add(BalloonTypes.GameTurns, x => x.GameTurns);
            _balloonTypes.Add(BalloonTypes.GameStartEnd, x => x.GameStartEnd);

            using (var reg = new BalloonRegistrySettings())
            {
                GameMode = reg.GameMode;
                GameStartEnd = reg.GameStartEnd;
                GameTurns = reg.GameTurns;
            }

            PropertyChanged += OnPropertyChanged;
        }

        public bool IsEnabled(string balloonType)
        {
            if (_balloonTypes.ContainsKey(balloonType))
            {
                return _balloonTypes[balloonType](this);
            }
            return false;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            using (var reg = new BalloonRegistrySettings())
            {
                reg.GameMode = GameMode;
                reg.GameStartEnd = GameStartEnd;
                reg.GameTurns = GameTurns;
            }
        }

        public bool GameMode
        {
            get { return _gameMode; }
            set
            {
                if (value.Equals(_gameMode))
                {
                    return;
                }
                _gameMode = value;
                NotifyOfPropertyChange(() => GameMode);
            }
        }

        public bool GameStartEnd
        {
            get { return _gameStartEnd; }
            set
            {
                if (value.Equals(_gameStartEnd))
                {
                    return;
                }
                _gameStartEnd = value;
                NotifyOfPropertyChange(() => GameStartEnd);
            }
        }

        public bool GameTurns
        {
            get { return _gameTurns; }
            set
            {
                if (value.Equals(_gameTurns))
                {
                    return;
                }
                _gameTurns = value;
                NotifyOfPropertyChange(() => GameTurns);
            }
        }
    }
}
