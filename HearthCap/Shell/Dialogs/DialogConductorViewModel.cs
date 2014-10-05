// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogConductorViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The dialog conductor view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Dialogs
{
    using System;
    using System.Collections;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    /// <summary>
    /// The dialog conductor view model.
    /// </summary>
    [Export(typeof(IDialogManager)), PartCreationPolicy(CreationPolicy.Shared)]
    public class DialogConductorViewModel : PropertyChangedBase, IDialogManager, IConductActiveItem
    {
        /// <summary>
        /// The create message box.
        /// </summary>
        readonly Func<IMessageBox> createMessageBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogConductorViewModel"/> class.
        /// </summary>
        /// <param name="messageBoxFactory">
        /// The message box factory.
        /// </param>
        [ImportingConstructor]
        public DialogConductorViewModel(Func<IMessageBox> messageBoxFactory)
        {
            this.createMessageBox = messageBoxFactory;
        }

        /// <summary>
        /// Gets the active item.
        /// </summary>
        public IScreen ActiveItem { get; private set; }

        /// <summary>
        /// The get children.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable GetChildren()
        {
            return this.ActiveItem != null ? new[] { this.ActiveItem } : new object[0];
        }

        /// <summary>
        /// The activate item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
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

        /// <summary>
        /// The deactivate item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="close">
        /// The close.
        /// </param>
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

        /// <summary>
        /// Gets or sets the active item.
        /// </summary>
        object IHaveActiveItem.ActiveItem
        {
            get { return this.ActiveItem; }
            set { this.ActivateItem(value); }
        }

        /// <summary>
        /// The activation processed.
        /// </summary>
        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate { };

        /// <summary>
        /// The show dialog.
        /// </summary>
        /// <param name="dialogModel">
        /// The dialog model.
        /// </param>
        public void ShowDialog(IScreen dialogModel)
        {
            this.ActivateItem(dialogModel);
        }

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

        /// <summary>
        /// The close active item core.
        /// </summary>
        void CloseActiveItemCore()
        {
            var oldItem = this.ActiveItem;
            this.ActivateItem(null);
            oldItem.Deactivate(true);
        }
    }
}