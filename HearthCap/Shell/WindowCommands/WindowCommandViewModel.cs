namespace HearthCap.Shell.WindowCommands
{
    using Caliburn.Micro;

    public abstract class WindowCommandViewModel : PropertyChangedBase, IWindowCommand
    {
        /// <summary>
        /// Creates an instance of <see cref="T:Caliburn.Micro.PropertyChangedBase"/>.
        /// </summary>
        protected WindowCommandViewModel()
        {
            this.Order = 1;
        }

        public int Order { get; set; }
    }
}