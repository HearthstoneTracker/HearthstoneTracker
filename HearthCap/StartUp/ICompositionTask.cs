// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositionTask.cs" company="">
//   
// </copyright>
// <summary>
//   The CompositionTask interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.StartUp
{
    using System.ComponentModel.Composition.Hosting;

    /// <summary>
    /// The CompositionTask interface.
    /// </summary>
    public interface ICompositionTask
    {
        /// <summary>
        /// The compose.
        /// </summary>
        /// <param name="batch">
        /// The batch.
        /// </param>
        void Compose(CompositionBatch batch);
    }
}