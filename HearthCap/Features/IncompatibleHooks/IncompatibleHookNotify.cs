// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncompatibleHookNotify.cs" company="">
//   
// </copyright>
// <summary>
//   The incompatible hooks notify.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.IncompatibleHooks
{
    using System.ComponentModel.Composition;
    using System.Windows;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.EngineEvents;
    using HearthCap.StartUp;

    /// <summary>
    /// The incompatible hooks notify.
    /// </summary>
    [Export(typeof(IStartupTask))]
    public class IncompatibleHooksNotify : IStartupTask, 
        IHandle<IncompatibleHooksFound>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The already fired.
        /// </summary>
        private bool alreadyFired;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncompatibleHooksNotify"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public IncompatibleHooksNotify(IEventAggregator events)
        {
            this.events = events;
            events.Subscribe(this);
        }

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(IncompatibleHooksFound message)
        {
            if (!this.alreadyFired)
            {
                this.alreadyFired = true;
                var msg = string.Format("The following software is conflicting with the screen capture:\n\n{0}\n\nPlease disable/quit that software or add Hearthstone to the software's ignore list then restart Hearthstone and the tracker.", message.Description);
                MessageBox.Show(msg, "Incompatible software found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}