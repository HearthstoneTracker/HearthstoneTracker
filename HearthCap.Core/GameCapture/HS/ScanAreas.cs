// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScanAreas.cs" company="">
//   
// </copyright>
// <summary>
//   The scan areas.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS
{
    using System.Collections.Generic;

    /// <summary>The scan areas.</summary>
    public class ScanAreas
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScanAreas"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ScanAreas()
        {
            this.BaseResolution = 900;
            this.Areas = new List<ScanArea>();
        }

        /// <summary>
        /// Gets or sets the base resolution.
        /// </summary>
        public int BaseResolution { get; set; }

        /// <summary>
        /// Gets or sets the areas.
        /// </summary>
        public List<ScanArea> Areas { get; set; }

        // public List<ScanArea> Heroes { get; set; }

        // public List<ScanArea> OpponentHeroes { get; set; }
    }
}