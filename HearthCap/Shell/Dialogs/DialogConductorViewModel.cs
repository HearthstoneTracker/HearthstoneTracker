namespace HearthCap.Shell.Dialogs
{
    using System;
    using System.Collections;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    [Export(typeof(IDialogManager)), PartCreationPolicy(CreationPolicy.Shared)]
    public class DialogConductorViewModel : PropertyChangedBase, IDialogManager, IConductActiveItem
    {
        readonly Func<IMessageBox> createMessageBox;

        [ImportingConstructor]
        public DialogConductorViewModel(Func<IMessageBox> messageBoxFactory)
        {
            this.createMessageBox = messageBoxFactory;
        }

        public IScreen ActiveItem { get; private set; }

        public IEnumerable GetChildren()
        {
            return this.ActiveItem != null ? new[] { this.ActiveItem } : new object[0];
        }

        public void ActivateItem(object item)
        {
            this.ActiveItem = item as IScreen;

            var child = this.ActiveItem as IChild;
            if (child != null)
                child.Parent = this;

            if (this.ActiveItem != null)
                this.ActiveItem.Activate();

            this.NotifyOfPropertyChange(() => this.ActiveItem);
            this.ActivationProcessed(this, new ActivationProcessedEventArgs { Item = this.ActiveItem, Success = true });
        }

        public void DeactivateItem(object item, bool close)
        {
            var guard = item as IGuardClose;
            if (guard != null)
            {
                guard.CanClose(result =>
                {
                    if (result)
                        this.CloseActiveItemCore();
                });
            }
            else this.CloseActiveItemCore();
        }

        object IHaveActiveItem.ActiveItem
        {
            get { return this.ActiveItem; }
            set { this.ActivateItem(value); }
        }

        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate { };

        public void ShowDialog(IScreen dialogModel)
        {
            this.ActivateItem(dialogModel);
        }

        public void ShowMessageBox(string message, string title = "Hello Screens", MessageBoxOptions options = MessageBoxOptions.Ok, Action<IMessageBox> callback = null)
        {
            var box = this.createMessageBox();

            box.DisplayName = title;
            box.Options = options;
            box.Message = message;

            if (callback != null)
                box.Deactivated += delegate { callback(box); };

            this.ActivateItem(box);
        }

        void CloseActiveItemCore()
        {
            var oldItem = this.ActiveItem;
            this.ActivateItem(null);
            oldItem.Deactivate(true);
        }
    }
}