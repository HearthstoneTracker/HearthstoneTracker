// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTransitionControl.cs" company="">
//   
// </copyright>
// <summary>
//   The custom transition control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Controls
{
    using System;

    using MahApps.Metro.Controls;

    /// <summary>
    /// The custom transition control.
    /// </summary>
    public class CustomTransitionControl : TransitioningContentControl
    {
        /// <summary>
        /// The content changed.
        /// </summary>
        public event EventHandler ContentChanged = delegate { };

        /// <summary>
        /// The on content changed.
        /// </summary>
        /// <param name="oldContent">
        /// The old content.
        /// </param>
        /// <param name="newContent">
        /// The new content.
        /// </param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            this.ContentChanged(this, EventArgs.Empty);
        }
    }
}