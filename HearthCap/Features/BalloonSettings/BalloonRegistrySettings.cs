using HearthCap.Shell.UserPreferences;

namespace HearthCap.Features.BalloonSettings
{
    public class BalloonRegistrySettings : RegistrySettings
    {
        public BalloonRegistrySettings()
            : base(@"Software\HearthstoneTracker\Balloons")
        {
        }

        public bool GameMode
        {
            get { return GetOrCreate("GameMode", true); }
            set { SetValue("GameMode", value); }
        }

        public bool GameTurns
        {
            get { return GetOrCreate("GameTurns", false); }
            set { SetValue("GameTurns", value); }
        }

        public bool GameStartEnd
        {
            get { return GetOrCreate("GameStartEnd", true); }
            set { SetValue("GameStartEnd", value); }
        }
    }
}
