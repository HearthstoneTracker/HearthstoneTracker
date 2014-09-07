namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System.Windows.Media;

    public class OverlayRegionModel : RegionModel
    {
        private double opacity;

        private Brush brush;

        public Brush Brush
        {
            get
            {
                return this.brush;
            }
            set
            {
                if (Equals(value, this.brush))
                {
                    return;
                }
                this.brush = value;
                this.NotifyOfPropertyChange(() => this.Brush);
            }
        }

        public double Opacity
        {
            get
            {
                return this.opacity;
            }
            set
            {
                if (value.Equals(this.opacity))
                {
                    return;
                }
                this.opacity = value;
                this.NotifyOfPropertyChange(() => this.Opacity);
            }
        }
    }
}