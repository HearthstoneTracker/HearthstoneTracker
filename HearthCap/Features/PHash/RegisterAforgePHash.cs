using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using HearthCap.StartUp;
using PHash;
using PHash.AForge;

namespace HearthCap.Features.PHash
{
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
