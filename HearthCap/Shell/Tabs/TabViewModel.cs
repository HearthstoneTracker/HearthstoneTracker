using Caliburn.Micro;

namespace HearthCap.Shell.Tabs
{
    public abstract class TabViewModel : Screen, ITab
    {
        public int Order { get; set; }

        public TabViewModel()
        {
            Order = 1;
        }
    }
}
