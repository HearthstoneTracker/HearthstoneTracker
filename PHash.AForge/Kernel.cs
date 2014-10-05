// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Kernel.cs" company="">
//   
// </copyright>
// <summary>
//   The kernel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PHash.AForge
{
    /// <summary>
    /// The kernel.
    /// </summary>
    public static class Kernel
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T[,]"/>.
        /// </returns>
        public static T[,] Create<T>(int width, int height, T value)
        {
            var m = new T[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    m[x, y] = value;
                }
            }

            return m;
        }
    }
}