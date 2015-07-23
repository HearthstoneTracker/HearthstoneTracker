namespace HearthCap.Shell
{
    using Caliburn.Micro;

    using HearthCap.Shell.Dialogs;

    public interface IShell : IConductActiveItem, IScreen
    {
        IDialogManager Dialogs { get; }

        void Show();
        
        void Hide();
    }
}