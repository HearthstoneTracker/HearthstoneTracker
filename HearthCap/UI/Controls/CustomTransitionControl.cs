namespace HearthCap.UI.Controls
{
    using System;

    using MahApps.Metro.Controls;

    public class CustomTransitionControl : TransitioningContentControl
    {
        public event EventHandler ContentChanged = delegate { };

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            this.ContentChanged(this, EventArgs.Empty);
        }
    }
}