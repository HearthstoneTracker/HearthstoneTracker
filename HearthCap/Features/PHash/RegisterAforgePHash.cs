// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterAforgePHash.cs" company="">
//   
// </copyright>
// <summary>
//   The register aforge p hash.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.PHash
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    using HearthCap.StartUp;

    using global::PHash;

    using global::PHash.AForge;

    /// <summary>
    /// The register aforge p hash.
    /// </summary>
    [Export(typeof(ICompositionTask))]
    public class RegisterAforgePHash : ICompositionTask
    {
        /// <summary>
        /// The compose.
        /// </summary>
        /// <param name="batch">
        /// The batch.
        /// </param>
        public void Compose(CompositionBatch batch)
        {
            batch.AddExportedValue<IPerceptualHash>(new AForgePerceptualHash());
            batch.AddExportedValue<ITemplateMatcher>(new Matcher());
            batch.AddExportedValue<ICornerDetector>(new CornerDetector());
        }
    }
}