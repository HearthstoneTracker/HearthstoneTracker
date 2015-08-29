using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Navigation;
using Caliburn.Micro;

namespace HearthCap.Shell
{
    [Export(typeof(CustomWindowManager))]
    public class CustomWindowManager : WindowManager
    {
        public Window MainWindow<T>(object context = null)
        {
            return CreateWindow(IoC.Get<T>(), false, context, null);
        }

        public Window EnsureWindow<T>(object context = null)
        {
            return CreateWindow(IoC.Get<T>(), false, context, null);
        }

        public override void ShowWindow(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            NavigationWindow navigationWindow = null;
            var current = Application.Current;
            if (current != null
                && current.MainWindow != null)
            {
                navigationWindow = current.MainWindow as NavigationWindow;
            }
            if (navigationWindow != null)
            {
                var page = CreatePage(rootModel, context, settings);
                navigationWindow.Navigate(page);
            }
            else
            {
                var window = CreateWindow(rootModel, false, context, settings);
                if (settings != null
                    && settings.ContainsKey("donotshow"))
                {
                }
                else
                {
                    window.Show();
                }
            }
        }
    }
}
