// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BalloonSettings.cs" company="">
//   
// </copyright>
// <summary>
//   The balloon types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.BalloonSettings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    /// <summary>
    /// The balloon types.
    /// </summary>
    public static class BalloonTypes
    {
        /// <summary>
        /// The game mode.
        /// </summary>
        public const string GameMode = "GameMode";

        /// <summary>
        /// The game turns.
        /// </summary>
        public const string GameTurns = "GameTurns";

        /// <summary>
        /// The game start end.
        /// </summary>
        public const string GameStartEnd = "GameStartEnd";
    }

    /// <summary>
    /// The balloon settings.
    /// </summary>
    [Export(typeof(BalloonSettings))]
    public class BalloonSettings : PropertyChangedBase
    {
        /// <summary>
        /// The game mode.
        /// </summary>
        private bool gameMode;

        /// <summary>
        /// The game start end.
        /// </summary>
        private bool gameStartEnd;

        /// <summary>
        /// The game turns.
        /// </summary>
        private bool gameTurns;

        /// <summary>
        /// The balloon types.
        /// </summary>
        private readonly IDictionary<string, Func<BalloonSettings, bool>> balloonTypes = new Dictionary<string, Func<BalloonSettings, bool>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BalloonSettings"/> class.
        /// </summary>
        public BalloonSettings()
        {
            this.balloonTypes.Add(BalloonTypes.GameMode, x => x.GameMode);
            this.balloonTypes.Add(BalloonTypes.GameTurns, x => x.GameTurns);
            this.balloonTypes.Add(BalloonTypes.GameStartEnd, x => x.GameStartEnd);

            using (var reg = new BalloonRegistrySettings())
            {
                this.GameMode = reg.GameMode;
                this.GameStartEnd = reg.GameStartEnd;
                this.GameTurns = reg.GameTurns;
            }

            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// The is enabled.
        /// </summary>
        /// <param name="balloonType">
        /// The balloon type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsEnabled(string balloonType)
        {
            if (this.balloonTypes.ContainsKey(balloonType))
            {
                return this.balloonTypes[balloonType](this);
            }

            return false;
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="propertyChangedEventArgs">
        /// The property changed event args.
        /// </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            using (var reg = new BalloonRegistrySettings())
            {
                reg.GameMode = this.GameMode;
                reg.GameStartEnd = this.GameStartEnd;
                reg.GameTurns = this.GameTurns;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether game mode.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether game start end.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether game turns.
        /// </summary>
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