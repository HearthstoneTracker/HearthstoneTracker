// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemplateMatcher.cs" company="">
//   
// </copyright>
// <summary>
//   The TemplateMatcher interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PHash
{
    using System.Drawing;

    /// <summary>
    /// The TemplateMatcher interface.
    /// </summary>
    public interface ITemplateMatcher
    {
        /// <summary>
        /// The is match.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <param name="threshold">
        /// The threshold.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        float IsMatch(Bitmap source, Bitmap template, float threshold = 0.90f);
    }
}