namespace HearthCap.Shell.Tabs
{
    using Caliburn.Micro;

    public abstract class TabViewModel : Screen, ITab
    {
        public int Order { get; set; }

        public TabViewModel()
        {
            this.Order = 1;
        }
    }
}