using System.Drawing;

namespace HearthCap.Core.GameCapture
{
    public interface IImageScanner
    {
        void Run(Bitmap img, object context);

        void Stop(object context);
    }
}
