// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IShell.cs" company="">
//   
// </copyright>
// <summary>
//   The Shell interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell
{
    using Caliburn.Micro;

    using HearthCap.Shell.Dialogs;

    /// <summary>
    /// The Shell interface.
    /// </summary>
    public interface IShell : IConductActiveItem, IScreen
    {
        /// <summary>
        /// Gets the dialogs.
        /// </summary>
        IDialogManager Dialogs { get; }

        /// <summary>
        /// The show.
        /// </summary>
        void Show();

        /// <summary>
        /// The hide.
        /// </summary>
        void Hide();
    }
}