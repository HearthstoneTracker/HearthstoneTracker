// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportCommandBarViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The support command bar view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Support
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.WindowCommands;

    /// <summary>
    /// The support command bar view model.
    /// </summary>
    [Export(typeof(IWindowCommand))]
    public class SupportCommandBarViewModel : WindowCommandViewModel
    {
        /// <summary>
        /// The dialog manager.
        /// </summary>
        private readonly IDialogManager dialogManager;

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The support view model.
        /// </summary>
        private readonly SupportViewModel supportViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportCommandBarViewModel"/> class.
        /// </summary>
        /// <param name="dialogManager">
        /// The dialog manager.
        /// </param>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="supportViewModel">
        /// The support view model.
        /// </param>
        [ImportingConstructor]
        public SupportCommandBarViewModel(
            IDialogManager dialogManager, 
            IEventAggregator events, 
            SupportViewModel supportViewModel)
        {
            this.Order = 95;
            this.dialogManager = dialogManager;
            this.events = events;
            this.supportViewModel = supportViewModel;
            this.events.Subscribe(this);
        }

        /// <summary>
        /// The support request.
        /// </summary>
        public void SupportRequest()
        {
            this.dialogManager.ShowDialog(this.supportViewModel);
        }
    }
}