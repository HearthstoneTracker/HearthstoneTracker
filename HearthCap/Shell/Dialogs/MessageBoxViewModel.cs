// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBoxViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The message box view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Dialogs
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    /// <summary>
    /// The message box view model.
    /// </summary>
    [Export(typeof(IMessageBox)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MessageBoxViewModel : Screen, IMessageBox
    {
        /// <summary>
        /// The selection.
        /// </summary>
        MessageBoxOptions selection;

        /// <summary>
        /// Gets a value indicating whether ok visible.
        /// </summary>
        public bool OkVisible
        {
            get { return this.IsVisible(MessageBoxOptions.Ok); }
        }

        /// <summary>
        /// Gets a value indicating whether cancel visible.
        /// </summary>
        public bool CancelVisible
        {
            get { return this.IsVisible(MessageBoxOptions.Cancel); }
        }

        /// <summary>
        /// Gets a value indicating whether yes visible.
        /// </summary>
        public bool YesVisible
        {
            get { return this.IsVisible(MessageBoxOptions.Yes); }
        }

        /// <summary>
        /// Gets a value indicating whether no visible.
        /// </summary>
        public bool NoVisible
        {
            get { return this.IsVisible(MessageBoxOptions.No); }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public MessageBoxOptions Options { get; set; }

        /// <summary>
        /// The ok.
        /// </summary>
        public void Ok()
        {
            this.Select(MessageBoxOptions.Ok);
        }

        /// <summary>
        /// The cancel.
        /// </summary>
        public void Cancel()
        {
            this.Select(MessageBoxOptions.Cancel);
        }

        /// <summary>
        /// The yes.
        /// </summary>
        public void Yes()
        {
            this.Select(MessageBoxOptions.Yes);
        }

        /// <summary>
        /// The no.
        /// </summary>
        public void No()
        {
            this.Select(MessageBoxOptions.No);
        }

        /// <summary>
        /// The was selected.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool WasSelected(MessageBoxOptions option)
        {
            return (this.selection & option) == option;
        }

        /// <summary>
        /// The is visible.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsVisible(MessageBoxOptions option)
        {
            return (this.Options & option) == option;
        }

        /// <summary>
        /// The select.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        private void Select(MessageBoxOptions option)
        {
            this.selection = option;
            this.TryClose();
        }
    }
}