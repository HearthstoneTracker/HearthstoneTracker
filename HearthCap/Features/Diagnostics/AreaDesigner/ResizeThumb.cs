using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += ResizeThumb_DragDelta;
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var region = DataContext as RegionModel;

            if (region != null)
            {
                double deltaVertical, deltaHorizontal;
                switch (VerticalAlignment)
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

                switch (HorizontalAlignment)
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
