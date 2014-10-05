// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Matcher.cs" company="">
//   
// </copyright>
// <summary>
//   The matcher.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PHash.AForge
{
    using System.Drawing;

    using global::AForge.Imaging;

    /// <summary>
    /// The matcher.
    /// </summary>
    public class Matcher : ITemplateMatcher
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
        public float IsMatch(Bitmap source, Bitmap template, float threshold = 0.80f)
        {
            var tm = new ExhaustiveTemplateMatching(threshold);
            var matchings = tm.ProcessImage(source, template);
            return matchings.Length > 0 ? matchings[0].Similarity : -1;
        }
    }
}