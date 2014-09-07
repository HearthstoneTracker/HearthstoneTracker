namespace PHash
{
    using System.Drawing;
    using System.Drawing.Imaging;

    public interface IPerceptualHash
    {
        ulong Create(Bitmap image);

        ulong Create(BitmapData image);
    }
}