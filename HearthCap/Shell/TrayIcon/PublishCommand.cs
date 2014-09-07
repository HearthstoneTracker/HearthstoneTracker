namespace HearthCap.Shell.TrayIcon
{
    using System;
    using System.Windows.Input;

    using Caliburn.Micro;

    public class PublishCommand<T> : ICommand
        where T : CommandEvent, new()
    {
        private readonly IEventAggregator events;

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
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            var ev = new T { Parameter = parameter };
            this.events.PublishOnUIThread(ev);
        }

        public event EventHandler CanExecuteChanged;
    }
}