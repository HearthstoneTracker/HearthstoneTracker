// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupTasks.cs" company="">
//   
// </copyright>
// <summary>
//   The startup tasks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Theme
{
    using System.ComponentModel.Composition;

    using HearthCap.Composition;
    using HearthCap.StartUp;

    /// <summary>
    /// The startup tasks.
    /// </summary>
    [Export(typeof(IStartupTask))]
    public class StartupTasks : IStartupTask
    {
        /// <summary>
        /// The service locator.
        /// </summary>
        private readonly IServiceLocator serviceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupTasks"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        [ImportingConstructor]
        public StartupTasks(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            var viewLocator = this.serviceLocator.GetInstance<IViewLocator>();
            Caliburn.Micro.ViewLocator.GetOrCreateViewType = viewLocator.GetOrCreateViewType;            
        }
    }
}