namespace HearthCap.Core.GameCapture
{
    using System.Drawing;

    public interface IImageScanner
    {
        void Run(Bitmap img, object context);
        
        void Stop(object context);
    }
}