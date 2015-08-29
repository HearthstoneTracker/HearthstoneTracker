using Caliburn.Micro;

namespace HearthCap.Shell.Tabs
{
    public interface ITab : IScreen
    {
        int Order { get; set; }
    }
}
