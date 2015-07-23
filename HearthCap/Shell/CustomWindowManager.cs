namespace HearthCap.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;

    using Caliburn.Micro;

    [Export(typeof(CustomWindowManager))]
    public class CustomWindowManager : WindowManager
    {
        public Window MainWindow<T>(object context = null)
        {
            return this.CreateWindow(IoC.Get<T>(), false, context, null);
        }

        public Window EnsureWindow<T>(object context = null)
        {
            return this.CreateWindow(IoC.Get<T>(), false, context, null);            
        }

        public override void ShowWindow(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            NavigationWindow navigationWindow = (NavigationWindow)null;
            Application current = Application.Current;
            if (current != null && current.MainWindow != null)
                navigationWindow = current.MainWindow as NavigationWindow;
            if (navigationWindow != null)
            {
                Page page = this.CreatePage(rootModel, context, settings);
                navigationWindow.Navigate((object)page);
            }
            else
            {
                var window = this.CreateWindow(rootModel, false, context, settings);
                if (settings != null && settings.ContainsKey("donotshow"))
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