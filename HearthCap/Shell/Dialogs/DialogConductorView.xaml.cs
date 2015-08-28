namespace HearthCap.Shell.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public partial class DialogConductorView : UserControl
    {
        bool disabled;

        public DialogConductorView()
        {
            InitializeComponent();
            // this.ActiveItem.ContentChanged += this.OnTransitionCompleted;
            this.Loaded += this.OnLoad;
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            if (this.disabled) this.DisableBackground();
        }

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

        public void EnableBackground()
        {
            this.disabled = false;
            this.ChangeEnabledState(this.GetBackground(), true);
        }

        public void DisableBackground()
        {
            this.disabled = true;
            this.ChangeEnabledState(this.GetBackground(), false);
        }

        IEnumerable<UIElement> GetBackground()
        {
            var contentControl = (ContentControl)this.Parent;
            var container = (Panel)contentControl.Parent;
            return container.Children.OfType<UIElement>().Where(child => child != contentControl);
        }

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