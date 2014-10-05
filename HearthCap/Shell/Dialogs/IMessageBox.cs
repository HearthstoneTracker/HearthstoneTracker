// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageBox.cs" company="">
//   
// </copyright>
// <summary>
//   The MessageBox interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Dialogs
{
    using Caliburn.Micro;

    /// <summary>
    /// The MessageBox interface.
    /// </summary>
    public interface IMessageBox : IScreen
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        MessageBoxOptions Options { get; set; }

        /// <summary>
        /// The ok.
        /// </summary>
        void Ok();

        /// <summary>
        /// The cancel.
        /// </summary>
        void Cancel();

        /// <summary>
        /// The yes.
        /// </summary>
        void Yes();

        /// <summary>
        /// The no.
        /// </summary>
        void No();

        /// <summary>
        /// The was selected.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool WasSelected(MessageBoxOptions option);
    }
}