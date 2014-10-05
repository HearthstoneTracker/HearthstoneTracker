// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublishCommand.cs" company="">
//   
// </copyright>
// <summary>
//   The publish command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.TrayIcon
{
    using System;
    using System.Windows.Input;

    using Caliburn.Micro;

    /// <summary>
    /// The publish command.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class PublishCommand<T> : ICommand
        where T : CommandEvent, new()
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishCommand{T}"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        public PublishCommand(IEventAggregator events)
        {
            this.events = events;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">
        /// Data used by the command.  If the command does not require data to be passed, this object can be set to null.
        /// </param>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command.  If the command does not require data to be passed, this object can be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            var ev = new T { Parameter = parameter };
            this.events.PublishOnUIThread(ev);
        }

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;
    }
}