namespace HearthCap.Shell.WindowCommands
{
    using Caliburn.Micro;

    public interface IWindowCommand : INotifyPropertyChangedEx
    {
        int Order { get; set; }
    }
}