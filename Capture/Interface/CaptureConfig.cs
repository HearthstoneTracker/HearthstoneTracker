using System;

namespace Capture.Interface
{
    [Serializable]
    public class CaptureConfig
    {
        #region Constructors and Destructors

        public CaptureConfig()
        {
            Direct3DVersion = Direct3DVersion.AutoDetect;
        }

        #endregion

        #region Public Properties

        public Direct3DVersion Direct3DVersion { get; set; }

        #endregion
    }
}
