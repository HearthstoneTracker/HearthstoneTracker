namespace HearthCap.Features.PHash
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    using global::PHash;
    using global::PHash.AForge;

    using HearthCap.StartUp;

    [Export(typeof(ICompositionTask))]
    public class RegisterAforgePHash : ICompositionTask
    {
        public void Compose(CompositionBatch batch)
        {
            batch.AddExportedValue<IPerceptualHash>(new AForgePerceptualHash());
            batch.AddExportedValue<ITemplateMatcher>(new Matcher());
            batch.AddExportedValue<ICornerDetector>(new CornerDetector());
        }
    }
}