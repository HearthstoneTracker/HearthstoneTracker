// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BalloonRegistrySettings.cs" company="">
//   
// </copyright>
// <summary>
//   The balloon registry settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.BalloonSettings
{
    using HearthCap.Shell.UserPreferences;

    /// <summary>
    /// The balloon registry settings.
    /// </summary>
    public class BalloonRegistrySettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BalloonRegistrySettings"/> class.
        /// </summary>
        public BalloonRegistrySettings()
            : base(@"Software\HearthstoneTracker\Balloons")
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether game mode.
        /// </summary>
        public bool GameMode
        {
            get
            {
                return this.GetOrCreate("GameMode", true);
            }

            set
            {
                this.SetValue("GameMode", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether game turns.
        /// </summary>
        public bool GameTurns
        {
            get
            {
                return this.GetOrCreate("GameTurns", false);
            }

            set
            {
                this.SetValue("GameTurns", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether game start end.
        /// </summary>
        public bool GameStartEnd
        {
            get
            {
                return this.GetOrCreate("GameStartEnd", true);
            }

            set
            {
                this.SetValue("GameStartEnd", value);
            }
        }
    }
}