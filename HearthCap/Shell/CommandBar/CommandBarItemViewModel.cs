namespace HearthCap.Shell.CommandBar
{
    using Caliburn.Micro;

    public class CommandBarItemViewModel : PropertyChangedBase, ICommandBarItem
    {
        public int Order { get; set; }
    }
}