namespace HearthCap.Shell.Dialogs
{
    using System;

    using Caliburn.Micro;

    public interface IDialogManager
    {
        void ShowDialog(IScreen dialogModel);

        void ShowMessageBox(string message, string title = null, MessageBoxOptions options = MessageBoxOptions.Ok, Action<IMessageBox> callback = null);
    }
}