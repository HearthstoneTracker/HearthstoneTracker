using System.Collections.Generic;
using System.Drawing;

namespace PHash
{
    public interface ICornerDetector
    {
        IList<Point> GetCorners(Bitmap bitmap);
    }
}
