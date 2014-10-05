// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The notifications view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Notifications
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    /// <summary>
    /// The notifications view model.
    /// </summary>
    [Export(typeof(NotificationsViewModel))]
    public class NotificationsViewModel : Conductor<NotificationViewModel>.Collection.AllActive, 
        IHandle<SendNotification>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public NotificationsViewModel(IEventAggregator events)
        {
            this.events = events;
            this.events.Subscribe(this);
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            // var vm = new NotificationViewModel(new SendNotification("test test", NotificationType.Info, 0));
            // Items.Add(vm);
            // ActivateItem(vm);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(SendNotification message)
        {
            // TODO: this is hacky...
            message.Message = string.Format("{0:HH:mm}: {1}", DateTime.Now, message.Message);
            Execute.OnUIThread(
                () =>
                    {
                        var vm = new NotificationViewModel(message);
                        this.Items.Add(vm);
                        this.ActivateItem(vm);
                        if (message.HideAfter > 0)
                        {
                            Task.Run(async () =>
                            {
                                await Task.Delay(message.HideAfter);
                                await Execute.OnUIThreadAsync(vm.CloseAnimated);
                            });
                        }                        
                    });
        }
    }

    /// <summary>
    /// The notification type.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// The info.
        /// </summary>
        Info, 

        /// <summary>
        /// The warning.
        /// </summary>
        Warning, 

        /// <summary>
        /// The error.
        /// </summary>
        Error
    }
}