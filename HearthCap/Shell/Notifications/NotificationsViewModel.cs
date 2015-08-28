namespace HearthCap.Shell.Notifications
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    [Export(typeof(NotificationsViewModel))]
    public class NotificationsViewModel : Conductor<NotificationViewModel>.Collection.AllActive,
        IHandle<SendNotification>
    {
        private readonly IEventAggregator events;

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
            //var vm = new NotificationViewModel(new SendNotification("test test", NotificationType.Info, 0));
            //Items.Add(vm);
            //ActivateItem(vm);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(SendNotification message)
        {
            // TODO: this is hacky...
            message.Message = String.Format("{0:HH:mm}: {1}", DateTime.Now, message.Message);
            Execute.OnUIThread(
                () =>
                    {
                        var vm = new NotificationViewModel(message);
                        Items.Add(vm);
                        ActivateItem(vm);
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

    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }
}