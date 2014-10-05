// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomWindowManager.cs" company="">
//   
// </copyright>
// <summary>
//   The custom window manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;

    using Caliburn.Micro;

    /// <summary>
    /// The custom window manager.
    /// </summary>
    [Export(typeof(CustomWindowManager))]
    public class CustomWindowManager : WindowManager
    {
        /// <summary>
        /// The main window.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Window"/>.
        /// </returns>
        public Window MainWindow<T>(object context = null)
        {
            return this.CreateWindow(IoC.Get<T>(), false, context, null);
        }

        /// <summary>
        /// The ensure window.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Window"/>.
        /// </returns>
        public Window EnsureWindow<T>(object context = null)
        {
            return this.CreateWindow(IoC.Get<T>(), false, context, null);            
        }

        /// <summary>
        /// The show window.
        /// </summary>
        /// <param name="rootModel">
        /// The root model.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public override void ShowWindow(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            NavigationWindow navigationWindow = null;
            Application current = Application.Current;
            if (current != null && current.MainWindow != null)
                navigationWindow = current.MainWindow as NavigationWindow;
            if (navigationWindow != null)
            {
                Page page = this.CreatePage(rootModel, context, settings);
                navigationWindow.Navigate(page);
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