using System.Drawing;
using System.Drawing.Imaging;

namespace PHash
{
    public interface IPerceptualHash
    {
        ulong Create(Bitmap image);

        ulong Create(BitmapData image);
    }
}
