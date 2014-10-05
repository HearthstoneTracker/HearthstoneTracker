// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverlayRegionModel.cs" company="">
//   
// </copyright>
// <summary>
//   The overlay region model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System.Windows.Media;

    /// <summary>
    /// The overlay region model.
    /// </summary>
    public class OverlayRegionModel : RegionModel
    {
        /// <summary>
        /// The opacity.
        /// </summary>
        private double opacity;

        /// <summary>
        /// The brush.
        /// </summary>
        private Brush brush;

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
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