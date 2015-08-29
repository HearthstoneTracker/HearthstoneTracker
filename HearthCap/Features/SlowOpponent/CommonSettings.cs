using HearthCap.Shell.UserPreferences;

namespace HearthCap.Features.SlowOpponent
{
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
