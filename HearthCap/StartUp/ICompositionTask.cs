namespace HearthCap.StartUp
{
    using System.ComponentModel.Composition.Hosting;

    public interface ICompositionTask
    {
        void Compose(CompositionBatch batch);
    }
}