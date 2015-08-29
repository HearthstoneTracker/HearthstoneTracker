using Caliburn.Micro;

namespace HearthCap.Shell.CommandBar
{
    public class CommandBarItemViewModel : PropertyChangedBase, ICommandBarItem
    {
        public int Order { get; set; }
    }
}
