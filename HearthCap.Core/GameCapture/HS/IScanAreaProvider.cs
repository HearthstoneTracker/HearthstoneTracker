namespace HearthCap.Core.GameCapture.HS
{
    using System.Collections.Generic;
    using System.Drawing;

    public interface IScanAreaProvider
    {
        IEnumerable<ScanAreas> GetScanAreas();

        void Load();

        Image GetImage(string name);
    }
}