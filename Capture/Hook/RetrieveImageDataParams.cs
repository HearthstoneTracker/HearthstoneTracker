namespace Capture.Hook
{
    using System;

    /// <summary>
    /// Used to hold the parameters to be passed to RetrieveImageData
    /// </summary>
    public struct RetrieveImageDataParams
    {
        #region Public Properties

        public byte[] Data { get; set; }

        public int Height { get; set; }

        public int Pitch { get; set; }

        public Guid RequestId { get; set; }

        public int Width { get; set; }

        #endregion
    }
}