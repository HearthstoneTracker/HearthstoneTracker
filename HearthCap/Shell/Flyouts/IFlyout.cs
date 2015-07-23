namespace HearthCap.Shell.Flyouts
{
    using Caliburn.Micro;

    using MahApps.Metro.Controls;

    public interface IFlyout : IScreen
    {
        string Header { get; set; }

        bool IsOpen { get; set; }

        Position Position { get; set; }

        string Name { get; set; }

        bool IsModal { get; set; }

        FlyoutTheme Theme { get; }
    }
}