// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewLocator.cs" company="">
//   
// </copyright>
// <summary>
//   The view locator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Theme
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    /// <summary>
    /// The view locator.
    /// </summary>
    [Export(typeof(IViewLocator))]
    public class ViewLocator : IViewLocator
    {
        /// <summary>
        /// The theme manager.
        /// </summary>
        private readonly IThemeManager themeManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocator"/> class.
        /// </summary>
        /// <param name="themeManager">
        /// The theme manager.
        /// </param>
        [ImportingConstructor]
        public ViewLocator(IThemeManager themeManager)
        {
            this.themeManager = themeManager;
        }

        /// <summary>
        /// The get or create view type.
        /// </summary>
        /// <param name="viewType">
        /// The view type.
        /// </param>
        /// <returns>
        /// The <see cref="UIElement"/>.
        /// </returns>
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