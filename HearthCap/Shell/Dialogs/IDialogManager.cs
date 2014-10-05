// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDialogManager.cs" company="">
//   
// </copyright>
// <summary>
//   The DialogManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Dialogs
{
    using System;

    using Caliburn.Micro;

    /// <summary>
    /// The DialogManager interface.
    /// </summary>
    public interface IDialogManager
    {
        /// <summary>
        /// The show dialog.
        /// </summary>
        /// <param name="dialogModel">
        /// The dialog model.
        /// </param>
        void ShowDialog(IScreen dialogModel);

        /// <summary>
        /// The show message box.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        void ShowMessageBox(string message, string title = null, MessageBoxOptions options = MessageBoxOptions.Ok, Action<IMessageBox> callback = null);
    }
}