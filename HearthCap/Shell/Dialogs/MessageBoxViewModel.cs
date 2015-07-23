namespace HearthCap.Shell.Dialogs
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    [Export(typeof(IMessageBox)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MessageBoxViewModel : Screen, IMessageBox
    {
        MessageBoxOptions selection;

        public bool OkVisible
        {
            get { return this.IsVisible(MessageBoxOptions.Ok); }
        }

        public bool CancelVisible
        {
            get { return this.IsVisible(MessageBoxOptions.Cancel); }
        }

        public bool YesVisible
        {
            get { return this.IsVisible(MessageBoxOptions.Yes); }
        }

        public bool NoVisible
        {
            get { return this.IsVisible(MessageBoxOptions.No); }
        }

        public string Message { get; set; }

        public MessageBoxOptions Options { get; set; }

        public void Ok()
        {
            this.Select(MessageBoxOptions.Ok);
        }

        public void Cancel()
        {
            this.Select(MessageBoxOptions.Cancel);
        }

        public void Yes()
        {
            this.Select(MessageBoxOptions.Yes);
        }

        public void No()
        {
            this.Select(MessageBoxOptions.No);
        }

        public bool WasSelected(MessageBoxOptions option)
        {
            return (this.selection & option) == option;
        }

        private bool IsVisible(MessageBoxOptions option)
        {
            return (this.Options & option) == option;
        }

        private void Select(MessageBoxOptions option)
        {
            this.selection = option;
            this.TryClose();
        }
    }
}