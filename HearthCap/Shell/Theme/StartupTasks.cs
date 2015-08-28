namespace HearthCap.Shell.Theme
{
    using System.ComponentModel.Composition;

    using HearthCap.Composition;
    using HearthCap.StartUp;

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
            var viewLocator = this.serviceLocator.GetInstance<IViewLocator>();
            Caliburn.Micro.ViewLocator.GetOrCreateViewType = viewLocator.GetOrCreateViewType;            
        }
    }
}