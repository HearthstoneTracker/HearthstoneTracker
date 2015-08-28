namespace HearthCap.Shell.Theme
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    using MahApps.Metro;

    [Export(typeof(IViewLocator))]
    public class ViewLocator : IViewLocator
    {
        private readonly IThemeManager themeManager;

        [ImportingConstructor]
        public ViewLocator(IThemeManager themeManager)
        {
            this.themeManager = themeManager;
        }

        public UIElement GetOrCreateViewType(Type viewType)
        {
            var cached = IoC.GetAllInstances(viewType).OfType<UIElement>().FirstOrDefault();
            if (cached != null)
            {
                Caliburn.Micro.ViewLocator.InitializeComponent(cached);
                return cached;
            }

            if (viewType.IsInterface || viewType.IsAbstract || !typeof(UIElement).IsAssignableFrom(viewType))
            {
                return new TextBlock { Text = string.Format("Cannot create {0}.", viewType.FullName) };
            }

            var newInstance = Activator.CreateInstance(viewType) as UIElement;
            var frameworkElement = newInstance as FrameworkElement;
            if (frameworkElement != null)
            {
                var resources = this.themeManager.GetThemeResources();
                foreach (var d in resources)
                {
                    frameworkElement.Resources.MergedDictionaries.Add(d);                    
                }
            }

            Caliburn.Micro.ViewLocator.InitializeComponent(newInstance);
            return newInstance;
        }
    }
}