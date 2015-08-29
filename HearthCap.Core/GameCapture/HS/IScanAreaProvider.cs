using System.Collections.Generic;
using System.Drawing;

namespace HearthCap.Core.GameCapture.HS
{
    public interface IScanAreaProvider
    {
        IEnumerable<ScanAreas> GetScanAreas();

        void Load();

        Image GetImage(string name);
    }
}
