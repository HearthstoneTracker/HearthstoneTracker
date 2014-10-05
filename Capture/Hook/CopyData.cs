// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyData.cs" company="">
//   
// </copyright>
// <summary>
//   The copy data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Hook
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The copy data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CopyData
    {
        /// <summary>
        /// The height.
        /// </summary>
        public int height;

        /// <summary>
        /// The width.
        /// </summary>
        public int width;

        /// <summary>
        /// The last rendered.
        /// </summary>
        public int lastRendered;

        /// <summary>
        /// The format.
        /// </summary>
        public int format;

        /// <summary>
        /// The pitch.
        /// </summary>
        public int pitch;

        /// <summary>
        /// The texture id.
        /// </summary>
        public Guid textureId;
    }
}