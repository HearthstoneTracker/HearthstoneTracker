namespace HearthCap.Shell.Tabs
{
    using Caliburn.Micro;

    public interface ITab : IScreen
    {
        int Order { get; set; }
    }
}