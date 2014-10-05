// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefServiceLocator.cs" company="">
//   
// </copyright>
// <summary>
//   The mef service locator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Composition
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;

    /// <summary>
    /// The mef service locator.
    /// </summary>
    [Export(typeof(IServiceLocator))]
    public class MefServiceLocator : IServiceLocator
    {
        /// <summary>
        /// The composition container.
        /// </summary>
        private readonly CompositionContainer compositionContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefServiceLocator"/> class.
        /// </summary>
        /// <param name="compositionContainer">
        /// The composition container.
        /// </param>
        [ImportingConstructor]
        public MefServiceLocator(CompositionContainer compositionContainer)
        {
            this.compositionContainer = compositionContainer;
        }

        /// <summary>
        /// The get instance.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T GetInstance<T>(string key = null) where T : class
        {
            return this.GetInstance(typeof(T), key) as T;
        }

        /// <summary>
        /// The get instance.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public object GetInstance(Type type, string key = null)
        {
            var str = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(type) : key;
            var obj = this.compositionContainer.GetExportedValues<object>(str).FirstOrDefault<object>();
            if (obj == null)
            {
                throw new InvalidOperationException("Could not locate any exported values for '{0}'.");
            }

            return obj;
        }
    }
}