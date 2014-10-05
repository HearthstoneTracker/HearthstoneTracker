// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityWithId.cs" company="">
//   
// </copyright>
// <summary>
//   The EntityWithId interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    /// <summary>
    /// The EntityWithId interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public interface IEntityWithId<out T>
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        T Id { get; }
    }
}