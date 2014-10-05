// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveThumb.cs" company="">
//   
// </copyright>
// <summary>
//   The move thumb.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// The move thumb.
    /// </summary>
    public class MoveThumb : Thumb
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveThumb"/> class.
        /// </summary>
        public MoveThumb()
        {
            this.DragDelta += this.MoveThumb_DragDelta;
        }

        /// <summary>
        /// The move thumb_ drag delta.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var region = this.DataContext as RegionModel;
            if (region != null)
            {
                region.XPos += (int)e.HorizontalChange;
                region.YPos += (int)e.VerticalChange;
            }
        }        
    }
}
