﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResizeThumb.cs" company="">
//   
// </copyright>
// <summary>
//   The resize thumb.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// The resize thumb.
    /// </summary>
    public class ResizeThumb : Thumb
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeThumb"/> class.
        /// </summary>
        public ResizeThumb()
        {
            this.DragDelta += this.ResizeThumb_DragDelta;
        }

        /// <summary>
        /// The resize thumb_ drag delta.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var region = this.DataContext as RegionModel;

            if (region != null)
            {
                double deltaVertical, deltaHorizontal;
                switch (this.VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, region.Height - region.MinHeight);
                        region.Height -= (int)deltaVertical;
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, region.Height - region.MinHeight);
                        region.YPos += (int)deltaVertical;
                        region.Height -= (int)deltaVertical;
                        break;
                }

                switch (this.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, region.Width - region.MinWidth);
                        region.XPos += (int)deltaHorizontal;
                        region.Width -= (int)deltaHorizontal;
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, region.Width - region.MinWidth);
                        region.Width -= (int)deltaHorizontal;
                        break;
                }
            }

            e.Handled = true;
        }
    }
}
