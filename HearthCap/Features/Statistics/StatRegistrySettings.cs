// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatRegistrySettings.cs" company="">
//   
// </copyright>
// <summary>
//   The stat registry settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Statistics
{
    using System;

    using HearthCap.Shell.UserPreferences;

    /// <summary>
    /// The stat registry settings.
    /// </summary>
    public class StatRegistrySettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatRegistrySettings"/> class.
        /// </summary>
        /// <param name="statViewModelType">
        /// The stat view model type.
        /// </param>
        public StatRegistrySettings(Type statViewModelType)
            : base(string.Format(@"Software\HearthstoneTracker\{0}", statViewModelType.Name))
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether show win ratio.
        /// </summary>
        public bool ShowWinRatio
        {
            get
            {
                return this.GetOrCreate("ShowWinRatio", true);
            }

            set
            {
                this.SetValue("ShowWinRatio", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show win ratio coin.
        /// </summary>
        public bool ShowWinRatioCoin
        {
            get
            {
                return this.GetOrCreate("ShowWinRatioCoin", true);
            }

            set
            {
                this.SetValue("ShowWinRatioCoin", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show win ratio no coin.
        /// </summary>
        public bool ShowWinRatioNoCoin
        {
            get
            {
                return this.GetOrCreate("ShowWinRatioNoCoin", true);
            }

            set
            {
                this.SetValue("ShowWinRatioNoCoin", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show wins.
        /// </summary>
        public bool ShowWins
        {
            get
            {
                return this.GetOrCreate("ShowWins", true);
            }

            set
            {
                this.SetValue("ShowWins", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show wins coin.
        /// </summary>
        public bool ShowWinsCoin
        {
            get
            {
                return this.GetOrCreate("ShowWinsCoin", true);
            }

            set
            {
                this.SetValue("ShowWinsCoin", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show wins no coin.
        /// </summary>
        public bool ShowWinsNoCoin
        {
            get
            {
                return this.GetOrCreate("ShowWinsNoCoin", true);
            }

            set
            {
                this.SetValue("ShowWinsNoCoin", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show total games.
        /// </summary>
        public bool ShowTotalGames
        {
            get
            {
                return this.GetOrCreate("ShowTotalGames", true);
            }

            set
            {
                this.SetValue("ShowTotalGames", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show total games by coin.
        /// </summary>
        public bool ShowTotalGamesByCoin
        {
            get
            {
                return this.GetOrCreate("ShowTotalGamesByCoin", true);
            }

            set
            {
                this.SetValue("ShowTotalGamesByCoin", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show played vs ratio.
        /// </summary>
        public bool ShowPlayedVsRatio
        {
            get
            {
                return this.GetOrCreate("ShowPlayedVsRatio", true);
            }

            set
            {
                this.SetValue("ShowPlayedVsRatio", value);
            }
        }

    }
}