using System.ComponentModel.Composition.Hosting;

namespace HearthCap.StartUp
{
    public interface ICompositionTask
    {
        void Compose(CompositionBatch batch);
    }
}
