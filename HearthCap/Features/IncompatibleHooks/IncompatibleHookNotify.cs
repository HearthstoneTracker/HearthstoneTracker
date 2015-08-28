namespace HearthCap.Features.IncompatibleHooks
{
    using System.ComponentModel.Composition;
    using System.Windows;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.EngineEvents;
    using HearthCap.StartUp;

    [Export(typeof(IStartupTask))]
    public class IncompatibleHooksNotify : IStartupTask,
        IHandle<IncompatibleHooksFound>
    {
        private readonly IEventAggregator events;

        private bool alreadyFired;

        [ImportingConstructor]
        public IncompatibleHooksNotify(IEventAggregator events)
        {
            this.events = events;
            events.Subscribe(this);
        }

        public void Run()
        {
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(IncompatibleHooksFound message)
        {
            if (!alreadyFired)
            {
                alreadyFired = true;
                var msg = string.Format("The following software is conflicting with the screen capture:\n\n{0}\n\nPlease disable/quit that software or add Hearthstone to the software's ignore list then restart Hearthstone and the tracker.", message.Description);
                MessageBox.Show(msg, "Incompatible software found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}