namespace HearthCap.Features.SlowOpponent
{
    using HearthCap.Shell.UserPreferences;

    public class CommonSettings : RegistrySettings
    {
        public CommonSettings()
            : this(string.Empty)
        {
        }

        public CommonSettings(string subsection)
            : base(@"Software\HearthstoneTracker" + (subsection.EndsWith("\\") ? subsection : subsection + "\\"))
        {
        }
    }
}