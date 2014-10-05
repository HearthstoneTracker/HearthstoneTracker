// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceLocator.cs" company="">
//   
// </copyright>
// <summary>
//   The ServiceLocator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Composition
{
    using System;

    /// <summary>
    /// The ServiceLocator interface.
    /// </summary>
    public interface IServiceLocator
    {
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
        T GetInstance<T>(string key = null) where T : class;

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
        object GetInstance(Type type, string key = null);
    }
}