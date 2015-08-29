using Caliburn.Micro;

namespace HearthCap.Shell.WindowCommands
{
    public abstract class WindowCommandViewModel : PropertyChangedBase, IWindowCommand
    {
        /// <summary>
        ///     Creates an instance of <see cref="T:Caliburn.Micro.PropertyChangedBase" />.
        /// </summary>
        protected WindowCommandViewModel()
        {
            Order = 1;
        }

        public int Order { get; set; }
    }
}
