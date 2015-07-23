namespace Capture.Interface
{
    using System;

    [Serializable]
    public class CaptureConfig
    {
        #region Constructors and Destructors

        public CaptureConfig()
        {
            this.Direct3DVersion = Direct3DVersion.AutoDetect;
        }

        #endregion

        #region Public Properties

        public Direct3DVersion Direct3DVersion { get; set; }

        #endregion
    }
}