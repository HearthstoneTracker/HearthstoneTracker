using HearthCap.Core.GameCapture;
using HearthCap.Shell.UserPreferences;

namespace HearthCap.Features.EngineControl
{
    public class EngineRegistrySettings : RegistrySettings
    {
        public EngineRegistrySettings()
            : base(@"Software\HearthstoneTracker\")
        {
        }

        public CaptureMethod CaptureMethod
        {
            get { return GetOrCreate("CaptureMethod", CaptureMethod.AutoDetect); }
            set { SetValue("CaptureMethod", value); }
        }

        public int Speed
        {
            get { return GetOrCreate("Speed", (int)Speeds.Default); }
            set { SetValue("Speed", value); }
        }

        public bool AutoStart
        {
            get { return GetOrCreate("AutoStart", 1) == 1; }
            set { SetValue("AutoStart", value ? 1 : 0); }
        }
    }
}
