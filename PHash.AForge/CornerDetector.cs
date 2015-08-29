using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AForge.Imaging;

namespace PHash.AForge
{
    public class CornerDetector : ICornerDetector
    {
        public IList<Point> GetCorners(Bitmap bitmap)
        {
            var mcd = new MoravecCornersDetector();
            var corners = mcd.ProcessImage(bitmap);

            return new List<Point>(corners.Select(x => new Point(x.X, x.Y)));
        }
    }
}
