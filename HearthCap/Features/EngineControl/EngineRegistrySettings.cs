namespace HearthCap.Features.EngineControl
{
    using HearthCap.Core.GameCapture;
    using HearthCap.Shell.UserPreferences;

    public class EngineRegistrySettings : RegistrySettings
    {
        public EngineRegistrySettings()
            : base(@"Software\HearthstoneTracker\")
        {
        }

        public CaptureMethod CaptureMethod
        {
            get
            {
                return GetOrCreate("CaptureMethod", CaptureMethod.AutoDetect);
            }
            set
            {
                SetValue("CaptureMethod", value);
            }
        }

        public int Speed
        {
            get
            {
                return GetOrCreate("Speed", (int)Speeds.Default);
            }
            set
            {
                SetValue("Speed", value);
            }
        }

        public bool AutoStart
        {
            get
            {
                return this.GetOrCreate("AutoStart", 1) == 1;
            }
            set
            {
                this.SetValue("AutoStart", value ? 1 : 0);
            }
        }
    }
}