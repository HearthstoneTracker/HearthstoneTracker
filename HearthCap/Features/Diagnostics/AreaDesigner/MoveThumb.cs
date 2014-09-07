namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            this.DragDelta += this.MoveThumb_DragDelta;
        }

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
