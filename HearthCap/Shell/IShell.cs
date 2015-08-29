using Caliburn.Micro;
using HearthCap.Shell.Dialogs;

namespace HearthCap.Shell
{
    public interface IShell : IConductActiveItem, IScreen
    {
        IDialogManager Dialogs { get; }

        void Show();

        void Hide();
    }
}
