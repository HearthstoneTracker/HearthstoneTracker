using Caliburn.Micro;

namespace HearthCap.Features.Settings
{
    public abstract class SettingsScreen : Screen, ISettingsScreen
    {
        public int Order { get; set; }
    }
}
