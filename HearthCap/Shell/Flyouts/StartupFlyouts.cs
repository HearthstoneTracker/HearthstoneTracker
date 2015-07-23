namespace HearthCap.Shell.Flyouts
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    using Caliburn.Micro;

    using HearthCap.Composition;
    using HearthCap.StartUp;

    using MahApps.Metro.Controls;

    [Export(typeof(IStartupTask))]
    public class StartupFlyouts : IStartupTask
    {
        private readonly IServiceLocator serviceLocator;

        [ImportingConstructor]
        public StartupFlyouts(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public void Run()
        {
            var getNamedElements = BindingScope.GetNamedElements;
            BindingScope.GetNamedElements = o =>
            {
                var metroWindow = o as MetroWindow;
                if (metroWindow == null)
                {
                    return getNamedElements(o);
                }

                var list = new List<FrameworkElement>(getNamedElements(o));
                var type = o.GetType();
                var fields =
                    o.GetType()
                     .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                     .Where(f => f.DeclaringType == type);
                var flyouts =
                    fields.Where(f => f.FieldType == typeof(FlyoutsControl))
                          .Select(f => f.GetValue(o))
                          .Cast<FlyoutsControl>();
                list.AddRange(flyouts);
                return list;
            };
        }
    }
}