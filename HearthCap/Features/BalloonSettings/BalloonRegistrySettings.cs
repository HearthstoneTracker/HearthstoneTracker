namespace HearthCap.Features.BalloonSettings
{
    using HearthCap.Shell.UserPreferences;

    public class BalloonRegistrySettings : RegistrySettings
    {
        public BalloonRegistrySettings()
            : base(@"Software\HearthstoneTracker\Balloons")
        {
        }

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