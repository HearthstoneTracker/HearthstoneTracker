// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScanAreaProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The ScanAreaProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// The ScanAreaProvider interface.
    /// </summary>
    public interface IScanAreaProvider
    {
        /// <summary>
        /// The get scan areas.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<ScanAreas> GetScanAreas();

        /// <summary>
        /// The load.
        /// </summary>
        void Load();

        /// <summary>
        /// The get image.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        Image GetImage(string name);
    }
}