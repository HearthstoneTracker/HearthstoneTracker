using System.Windows.Media;

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    public class OverlayRegionModel : RegionModel
    {
        private double opacity;

        private Brush brush;

        public Brush Brush
        {
            get { return brush; }
            set
            {
                if (Equals(value, brush))
                {
                    return;
                }
                brush = value;
                NotifyOfPropertyChange(() => Brush);
            }
        }

        public double Opacity
        {
            get { return opacity; }
            set
            {
                if (value.Equals(opacity))
                {
                    return;
                }
                opacity = value;
                NotifyOfPropertyChange(() => Opacity);
            }
        }
    }
}
