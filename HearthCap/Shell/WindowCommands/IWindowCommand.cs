using Caliburn.Micro;

namespace HearthCap.Shell.WindowCommands
{
    public interface IWindowCommand : INotifyPropertyChangedEx
    {
        int Order { get; set; }
    }
}
