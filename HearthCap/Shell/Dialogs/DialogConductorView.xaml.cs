// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogConductorView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The dialog conductor view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The dialog conductor view.
    /// </summary>
    public partial class DialogConductorView
    {
        /// <summary>
        /// The disabled.
        /// </summary>
        bool disabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogConductorView"/> class.
        /// </summary>
        public DialogConductorView()
        {
            this.InitializeComponent();

            // this.ActiveItem.ContentChanged += this.OnTransitionCompleted;
            this.Loaded += this.OnLoad;
        }

        /// <summary>
        /// The on load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void OnLoad(object sender, RoutedEventArgs e)
        {
            if (this.disabled) this.DisableBackground();
        }

        /// <summary>
        /// The on transition completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void OnTransitionCompleted(object sender, EventArgs e)
        {
            if (this.ActiveItem.Content == null)
                this.EnableBackground();
            else
            {
                this.DisableBackground();

                var control = this.ActiveItem.Content as Control;
                if (control != null)
                    control.Focus();
            }
        }

        /// <summary>
        /// The enable background.
        /// </summary>
        public void EnableBackground()
        {
            this.disabled = false;
            this.ChangeEnabledState(this.GetBackground(), true);
        }

        /// <summary>
        /// The disable background.
        /// </summary>
        public void DisableBackground()
        {
            this.disabled = true;
            this.ChangeEnabledState(this.GetBackground(), false);
        }

        /// <summary>
        /// The get background.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<UIElement> GetBackground()
        {
            var contentControl = (ContentControl)this.Parent;
            var container = (Panel)contentControl.Parent;
            return container.Children.OfType<UIElement>().Where(child => child != contentControl);
        }

        /// <summary>
        /// The change enabled state.
        /// </summary>
        /// <param name="background">
        /// The background.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        void ChangeEnabledState(IEnumerable<UIElement> background, bool state)
        {
            foreach (var uiElement in background)
            {
                var control = uiElement as Control;
                if (control != null)
                    control.IsEnabled = state;
                else
                {
                    var panel = uiElement as Panel;
                    if (panel != null)
                    {
                        foreach (UIElement child in panel.Children)
                        {
                            this.ChangeEnabledState(new[] { child }, state);
                        }
                    }
                }
            }
        }
    }
}