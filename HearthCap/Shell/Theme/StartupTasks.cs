using System.ComponentModel.Composition;
using HearthCap.Composition;
using HearthCap.StartUp;

namespace HearthCap.Shell.Theme
{
    [Export(typeof(IStartupTask))]
    public class StartupTasks : IStartupTask
    {
        private readonly IServiceLocator serviceLocator;

        [ImportingConstructor]
        public StartupTasks(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public void Run()
        {
            var viewLocator = serviceLocator.GetInstance<IViewLocator>();
            Caliburn.Micro.ViewLocator.GetOrCreateViewType = viewLocator.GetOrCreateViewType;
        }
    }
}
