namespace HearthCap.Composition
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;

    [Export(typeof(IServiceLocator))]
    public class MefServiceLocator : IServiceLocator
    {
        private readonly CompositionContainer compositionContainer;

        [ImportingConstructor]
        public MefServiceLocator(CompositionContainer compositionContainer)
        {
            this.compositionContainer = compositionContainer;
        }

        public T GetInstance<T>(string key = null) where T : class
        {
            return GetInstance(typeof(T), key) as T;
        }

        public object GetInstance(Type type, string key = null)
        {
            var str = String.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(type) : key;
            var obj = this.compositionContainer.GetExportedValues<object>(str).FirstOrDefault<object>();
            if (obj == null)
            {
                throw new InvalidOperationException("Could not locate any exported values for '{0}'.");
            }

            return obj;
        }
    }
}