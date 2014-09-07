namespace PHash
{
    using System.Collections.Generic;
    using System.Drawing;

    public interface ICornerDetector
    {
        IList<Point> GetCorners(Bitmap bitmap);
    }
}