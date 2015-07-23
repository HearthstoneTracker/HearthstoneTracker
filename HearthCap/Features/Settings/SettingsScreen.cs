namespace HearthCap.Features.Settings
{
    using Caliburn.Micro;

    public abstract class SettingsScreen : Screen, ISettingsScreen
    {
        public int Order { get; set; }
    }
}