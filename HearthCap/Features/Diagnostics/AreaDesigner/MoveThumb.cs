using System.Windows.Controls.Primitives;

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += MoveThumb_DragDelta;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var region = DataContext as RegionModel;
            if (region != null)
            {
                region.XPos += (int)e.HorizontalChange;
                region.YPos += (int)e.VerticalChange;
            }
        }
    }
}
